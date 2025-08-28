# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [3.0.0] - Not Yet Released

### Changed
- Removed the ability to search for an existing `Deconstruct()` extension method (issue [#24](https://github.com/JasonBock/AutoDeconstruct/issues/24))

## [2.0.0] - 2025.08.26

### Added
- Added a refactoring to add attributes to code (issue [#22](https://github.com/JasonBock/AutoDeconstruct/issues/22))
- `[TargetAutoDeconstruct]` can now be used to target any type (issue [#19](https://github.com/JasonBock/AutoDeconstruct/issues/19) and issue [#21](https://github.com/JasonBock/AutoDeconstruct/issues/21))

### Changed
- Removed the ability to add deconstruction to every type in an assembly (issue [#18](https://github.com/JasonBock/AutoDeconstruct/issues/18))
- `Deconstruct()` parameters are now sorted by property name (issue [#23](https://github.com/JasonBock/AutoDeconstruct/issues/23))

## [1.1.0] - 2025.02.01

### Added
- AutoDeconstruct no longer automatically scans for types; `[AutoDeconstruct]` must be used (this improves source generator performance) (issue [#16](https://github.com/JasonBock/AutoDeconstruct/issues/16))

## [1.0.0] - 2022.11.25

### Added
- Generated code now uses fully-qualified named (issue [#5](https://github.com/JasonBock/AutoDeconstruct/issues/5))
- Records are now considered for `Deconstruct()` generation (issue [#4](https://github.com/JasonBock/AutoDeconstruct/issues/4))
- Putting `[NoAutoDeconstruct]` on a type will signal AutoDeconstruct not to generate a `Deconstruct()` method (issue [#6](https://github.com/JasonBock/AutoDeconstruct/issues/6))
- All parameter names now have `@` in front to eliminate any potential keyword conflicts (issue [#9](https://github.com/JasonBock/AutoDeconstruct/issues/9))

### Changed
- Updated documentation (issue [#7](https://github.com/JasonBock/AutoDeconstruct/issues/7))
- Generated code is now stored in one file (issue [#2](https://github.com/JasonBock/AutoDeconstruct/issues/2))

### Fixed
- Generic types are now handled correctly (issue [#8](https://github.com/JasonBock/AutoDeconstruct/issues/8))