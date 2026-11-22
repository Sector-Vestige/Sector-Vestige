// From https://github.com/DeltaV-Station/Delta-v/
// Dependencies
const fs = require("fs");
const yaml = require("js-yaml");
const axios = require("axios");

// Use GitHub token if available
if (process.env.GITHUB_TOKEN) axios.defaults.headers.common["Authorization"] = `Bearer ${process.env.GITHUB_TOKEN}`;

// Regexes
const HeaderRegex = /^[ \t]*(?::cl:|🆑)[ \t]*(.*)$/im; // :cl: or 🆑 on its own line, with an optional author behind it [0]
const AuthorRegex = /^[A-Za-z0-9_-]{1,39}(?:[ \t]*,[ \t]*[A-Za-z0-9_-]{1,39})*$/; // one or more usernames, comma separated
const EntryRegex = /^ *[*-]? *(add|remove|tweak|fix): *([^\n\r]+)\r?$/img; // * or - followed by change type [0] and change message [1]
const CommentRegex = /<!--.*?-->/gs; // HTML comments

const CHANGE_TYPES = { add: "Add", remove: "Remove", tweak: "Tweak", fix: "Fix" };

// Main function
async function main() {
    const { GITHUB_REPOSITORY, PR_NUMBER, CHANGELOG_DIR } = process.env;
    if (!GITHUB_REPOSITORY || !PR_NUMBER || !CHANGELOG_DIR) {
        throw new Error("GITHUB_REPOSITORY, PR_NUMBER and CHANGELOG_DIR must all be set.");
    }

    // Get PR details
    const { merged_at, body, user } = await fetchPullRequest(GITHUB_REPOSITORY, PR_NUMBER);

    if (!merged_at) {
        console.log("Pull request was not merged, skipping");
        return;
    }

    // Remove comments from the body
    const commentlessBody = (body || "").replace(CommentRegex, "");

    // Get author
    const headerMatch = HeaderRegex.exec(commentlessBody);
    if (!headerMatch) {
        console.log("No changelog entry found, skipping");
        return;
    }

    let author = (headerMatch[1] || "").trim();
    if (!author) {
        console.log("No author found, setting it to author of the PR");
        author = user.login;
    } else if (!AuthorRegex.test(author)) {
        console.log(`Header author ${JSON.stringify(author)} is not a username list, setting it to author of the PR`);
        author = user.login;
    }

    // Get all changes from the body
    const entries = getChanges(commentlessBody);
    if (entries.length === 0) {
        console.log("Changelog header found, but no add/remove/tweak/fix entries under it, skipping");
        return;
    }

    // Time is something like 2021-08-29T20:00:00Z
    // Time should be something like 2023-02-18T00:00:00.0000000+00:00
    const time = merged_at.replace(/z$/i, ".0000000+00:00");

    // Construct changelog yml entry
    const entry = {
        author: author,
        changes: entries,
        id: getHighestCLNumber() + 1,
        time: time,
    };

    // Write changelogs
    writeChangelog(entry);

    console.log(`Changelog updated with changes from PR #${PR_NUMBER}:`);
    console.log(yaml.dump([entry], { indent: 2 }));
}


// Code chunking

// Fetch the pull request, failing loudly rather than exiting 0 without a changelog
async function fetchPullRequest(repository, prNumber) {
    try {
        const response = await axios.get(`https://api.github.com/repos/${repository}/pulls/${prNumber}`);
        return response.data;
    } catch (err) {
        const status = err.response && err.response.status;
        const hint = status === 403 || status === 429
            ? " This is usually the unauthenticated rate limit — is GITHUB_TOKEN passed to this step?"
            : "";
        throw new Error(`Could not fetch PR #${prNumber} (HTTP ${status || "no response"}).${hint}`);
    }
}

// Get all changes from the PR body
function getChanges(body) {
    const entries = [];

    for (const match of body.matchAll(EntryRegex)) {
        const type = CHANGE_TYPES[match[1].toLowerCase()];
        const message = match[2].trim();

        if (type && message) {
            entries.push({
                type: type,
                message: message,
            });
        }
    }

    return entries;
}

// Read the existing entries, tolerating a missing or empty file
function readEntries() {
    const path = `../../${process.env.CHANGELOG_DIR}`;

    if (!fs.existsSync(path)) return [];

    const data = yaml.load(fs.readFileSync(path, "utf8"));
    return data && Array.isArray(data.Entries) ? data.Entries : [];
}

// Get the highest changelog number from the changelogs file
function getHighestCLNumber() {
    return readEntries().reduce((highest, entry) => Math.max(highest, entry.id || 0), 0);
}

function writeChangelog(entry) {
    const entries = readEntries();
    entries.push(entry);

    // Write updated changelogs file
    fs.writeFileSync(
        `../../${process.env.CHANGELOG_DIR}`,
        "Name: Sector-vestige\nOrder: -1\nEntries:\n" + // IF YOU ARE A FORK, CHANGE THIS!!!!!!!!!!!!
            yaml.dump(entries, { indent: 2 }).replace(/^---/, "")
    );
}

// Run main
main().catch((err) => {
    console.error(`::error::${err.message}`);
    process.exit(1);
});
