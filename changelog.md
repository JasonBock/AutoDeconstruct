# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - Not Released Yest

### Added
- Generated code now uses fully-qualified named (issue [#5](https://github.com/JasonBock/AutoDeconstruct/issues/5))
- Records are now considered for `Deconstruct()` generation (issue [#4](https://github.com/JasonBock/AutoDeconstruct/issues/4))
- Putting `[NoAutoDeconstruct]` on a type will signal AutoDeconstruct not to generate a `Deconstruct()` method (issue [#6](https://github.com/JasonBock/AutoDeconstruct/issues/6))

### Fixed
- Generic types are now handled correctly (issue [#8](https://github.com/JasonBock/AutoDeconstruct/issues/8))