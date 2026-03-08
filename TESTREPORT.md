# Test Report

## Testing

- [*] Double click extension to install. (1)
- [*] Extension appears in **Extensions > Manage Extensions** (2)
- [*] **View > Other Windows > GitHub Info** menu item is present (3)
- [*] Opening a solution with a GitHub remote shows issues and PRs (4)
- [*] Closing all solutions updates the tool window within ~3 seconds (5)
- [*] PAT configuration in **Tools > Options** works correctly
- [*] Auto-refresh fires at the configured interval
- [*] Solution switch detection updates the displayed repository
- [*] Manual **Refresh** button reloads data immediately
- [*] UI respects the current VS theme (dark/light/blue)

(1) - i can also choose vs2022 to install, it should only be vs2026.
(2) - It can use some more documentation about how to use it.
(3) - Is it possible to let it appear in the already available Git menu?
(4)(5) - Yes, when PAT is confgured.
(6) - The first time, the options are not there, but after closing
VS2026 and reopen it, it is there. Tried it multiple times, everytime
the same outcome.

Remarks,
Is it possible when uninstalling you can also clear the PAT?
