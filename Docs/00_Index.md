# SmartSurvey Script Documentation — Index

> **Source:** `DBI Scripting\Forms\Scripting\FrmBuildScript.xaml.cs`
> **Purpose:** The script compiler parses `.q` script files and produces SQLite `.db` databases
> for the SmartSurvey mobile/web data collection platform.

---

## Documents

| # | Document | Covers |
|---|----------|--------|
| 1 | [Question Types](01_Question_Types.md) | All 28 question types, their keywords, syntax, modifiers, and usage notes |
| 2 | [Logic System](02_Logic_System.md) | Conditional logic (`*IF`, `*GOTO`, `*MSG`, `*INCLUDE`, `*EXCLUDE`), all logic functions, condition syntax |
| 3 | [Functions Reference](03_Functions_Reference.md) | All C# methods in the compiler, data structures, error messages |
| 4 | [Script Structure](04_Script_Structure.md) | File format, sections, ordering rules, comments, lists, language sections, full annotated template, common mistakes |
| 5 | [Attribute Modifiers](05_Attribute_Modifiers.md) | Every question-level and attribute-level modifier — rotation, layout, DKCS, DUMMY patterns, FORM sub-types, GPS, grouping, DB field mapping |
| 6 | [Multi-Language Guide](06_Multi_Language_Guide.md) | `@LANGUAGE` sections, allowed vs forbidden elements, language-to-DB field mapping, `*DKCS`/`*USELIST` in language sections, fallback behaviour, bilingual annotated example, common mistakes |
| 7 | [Database Schema](07_Database_Schema.md) | All 8 tables (columns, types, flag values, QType/LogicTypeId/FilterType enumerations), table relationships, sentinel row behaviour, useful query patterns |
| 8 | [Repeat Patterns](08_Repeat_Patterns.md) | Block REPEAT (`*REPEAT [source]`…`*ENDREPEAT`), inline REPEAT on `*QUESTION`, `?R` placeholder, source types (range vs QId), two-pass expansion, curly-brace `{QId}`/`{QId.N}` references, language section REPEAT, error reference, 5 worked examples |

---

## Quick Syntax Card

```
# Script header
PROJECT NAME: My Survey
PROJECT CODE: P001
SCRIPT VERSION: 1.0
SCRIPT NAME: MySurveyDB
SCRIPTED BY: John

# Define a shared option list
*LIST "BrandList"
1:Brand A
2:Brand B
3:Brand C

# Define a grid column list
*GRIDLIST "ScaleList"
1:Strongly Agree
2:Agree
3:Neutral
4:Disagree
5:Strongly Disagree

# Single response question
*QUESTION Q1 *SR *ROT
Which brand do you use most often?
*USELIST "BrandList"

# Multiple response with max
*QUESTION Q2 *MR *MAX 3 *IF [Q1=1]
Which other brands have you tried?
*USELIST "BrandList"

# Skip logic
*IF [Q1=3] *GOTO ScreenOut

# Attribute filter
*IF [Q1=1] *INCLUDE Q2 [1;2;3]

# Grid question
*QUESTION Q3 *GRIDSR *USEGRIDLIST "ScaleList"
Please rate each brand.
1:Brand A
2:Brand B

# Open ended
*QUESTION Q4 *OPEN
Any other comments?

# FIFS (mandatory)
*QUESTION FIFSInfo *FIFS
Field Interviewer & Supervisor Information

# Survey end
*QUESTION SurveyEnd *END
Thank you for completing the survey!

# Language section
@LANGUAGE "Bengali"
*QUESTION Q1 *SR
আপনি কোন ব্র্যান্ড সবচেয়ে বেশি ব্যবহার করেন?
1:ব্র্যান্ড এ
2:ব্র্যান্ড বি
3:ব্র্যান্ড সি
```

---

## Mandatory Questions

Every script must contain these QIds:

| QId | Type | Purpose |
|-----|------|---------|
| `RespName` | Any | Respondent name |
| `RespMobile` | Any | Respondent mobile |
| `Centre` | Any | Centre/location |
| `FIFSInfo` | `*FIFS` | Field team details |

---

## Output Database Tables

| Table | Contents |
|-------|---------|
| `T_ProjectInfo` | Project metadata |
| `T_Question` | One row per question |
| `T_OptAttribute` | Options/attributes per question |
| `T_GridInfo` | Grid row definitions |
| `T_LogicTable` | GOTO and MSG rules |
| `T_LogicAuto` | INCLUDE/EXCLUDE filter rules |
| `T_OptAttrbFilter` | Question-level attribute filters |
