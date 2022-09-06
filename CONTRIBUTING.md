# Contributing
This repository accepts contributions via Pull Requests.

You are encouraged to discuss changes with @juh9870#8970 in #ehce channel on the [official Discord server](https://discordapp.com/invite/yFFvF7m)

> Only bug fixes and performance improvements are accepted to this repository. If you want to add a feature,
> please discuss it with juh9870 on discord

## Commit namings
If you want your Pull Requests to be accepted, please name your commits according to [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/) standard.

> These messages may be hard to write by hand, so there are convenient tools to help with it:
> - Online generator: https://commitlint.io/
> - JetBrains IDE plugin: https://plugins.jetbrains.com/plugin/13389-conventional-commit
> - VSCode plugin: https://marketplace.visualstudio.com/items?itemName=vivaxy.vscode-conventional-commits

### Acceptable commit types:
| Commit type | description                                                                                                |
|-------------|------------------------------------------------------------------------------------------------------------|
| `feat`      | A new feature                                                                                              |
| `fix`       | A bug fix                                                                                                  |
| `perf`      | A code change that improves performance                                                                    |
| `test`      | Adding missing tests or correcting existing tests. Fixing bugs inside tests also falls under this category |
| `docs`      | Documentation only changes                                                                                 |
| `style`     | Changes that do not affect the meaning of the code (white-space, formatting, missing commas, etc)          |
| `build`     | Changes that only affect Unity builds                                                                      |
| `ci`        | Changes to CI configuration files and scripts                                                              |
| `revert`    | Revert of a commit                                                                                         |
| `chore`     | Changes that don't fall under other categories                                                             |

### Acceptable commit scopes:
| Scope name | description                                                         | applicable types                      |
|------------|---------------------------------------------------------------------|---------------------------------------|
| `gameplay` | Changes related to the non-combat gameplay or game logic            | `fix`, `feat`, `perf`                 |
| `combat`   | Changes related to the game combat system logic                     | `fix`, `feat`, `perf`                 |
| `modding`  | Changes related to modding                                          | `fix`, `feat`, `perf`, `test`, `docs` |
| `ui`       | Changes related to user interface that do not affect game logic     | `fix`, `feat`, `perf`                 |
| `visual`   | Changes that affect game visual that do not affect game logic or UI | `fix`, `feat`, `perf`                 |
| `misc`     | Changes that do not fall into any other category                    | all of them                           |

If you feel unsure about which options to pick, fell free to ping @juh9870#8970 in #ehce channel on the [official Discord server](https://discordapp.com/invite/yFFvF7m)
