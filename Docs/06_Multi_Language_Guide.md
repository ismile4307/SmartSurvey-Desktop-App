# Part 6 — Multi-Language Scripting Guide

> **Source:** `DBI Scripting\Forms\Scripting\FrmBuildScript.xaml.cs`
> **Relevant methods:** `ReadLanguageSection`, `prepareQuestionForLanguage`, `updateBengaliTranslation`, `update3rdTranslation` … `update9thTranslation`

---

## 1. Overview

SmartSurvey supports up to **9 additional languages** on top of the base English script. Each language is provided as a separate block appended to the bottom of the `.q` file, delimited by an `@LANGUAGE` marker.

The compiler reads the English body first to build the complete question/attribute structure, then reads each `@LANGUAGE` section to collect translated text, and finally writes the translations into the SQLite database alongside the English content.

---

## 2. `@LANGUAGE` Section Marker

```
@LANGUAGE "LanguageName"
```

| Element | Detail |
|---------|--------|
| Keyword | `@LANGUAGE` — must start at column 0 (no leading spaces) |
| Name | Free-text string in **double quotes**; used only as a label for the scripter |
| Position | Always **after** the English body; multiple `@LANGUAGE` blocks are ordered sequentially |
| Maximum | 9 language sections (Language 1 through Language 9) |
| Section end | The next `@LANGUAGE` line, or end of file |

**Example:**

```
@LANGUAGE "Bengali"
... Bengali translations ...

@LANGUAGE "Hindi"
... Hindi translations ...
```

---

## 3. Language-to-Database Field Mapping

The compiler does **not** use the language name string to decide which DB column to write. It uses the **ordinal position** of the `@LANGUAGE` block:

| Language ordinal | `T_Question` column | `T_OptAttribute` column | `T_GridInfo` column |
|-----------------|---------------------|-------------------------|---------------------|
| Language 1 (first `@LANGUAGE`) | `QuestionBengali` | `AttributeBengali` | `AttributeBengali` |
| Language 2 | `QuestionLang3` | `AttributeLang3` | `AttributeLang3` |
| Language 3 | `QuestionLang4` | `AttributeLang4` | `AttributeLang4` |
| Language 4 | `QuestionLang5` | `AttributeLang5` | `AttributeLang5` |
| Language 5 | `QuestionLang6` | `AttributeLang6` | `AttributeLang6` |
| Language 6 | `QuestionLang7` | `AttributeLang7` | `AttributeLang7` |
| Language 7 | `QuestionLang8` | `AttributeLang8` | `AttributeLang8` |
| Language 8 | `QuestionLang9` | `AttributeLang9` | `AttributeLang9` |
| Language 9 | `QuestionLang10` | `AttributeLang10` | `AttributeLang10` |

> **Note on naming:** The first language always writes to the `Bengali`-named columns regardless of the name you put in quotes. The second language writes to `Lang3` (not `Lang2`) — this is a historical quirk in the schema. `Lang2` does not exist as a column.

---

## 4. What a Language Section Contains

A language section is a **translation-only** mirror of the English body. It contains the same questions in the same order, but provides only translated text. **No structural information is re-specified.**

### 4.1 Allowed Elements

| Element | Syntax | Notes |
|---------|--------|-------|
| `*LIST` definition | `*LIST "ListName"` … `N:translated label` | Provides translated option labels for a named list |
| `*GRIDLIST` definition | `*GRIDLIST "ListName"` … `N:translated label` | Provides translated grid column labels |
| `*QUESTION` header | `*QUESTION QId` | QId must match an English question exactly |
| Question text | Line(s) following the header, before any attribute | Translated question wording |
| Attribute lines | `N:translated label` | Translated option/attribute text only |
| `*USELIST "name"` | In the attribute body | Pulls translated labels from a language-section `*LIST` |
| `*DKCS` | On the `*QUESTION` header line | Provides translated "Don't Know / Can't Say" label |
| `*END` / `*TERMINATE` | On the `*QUESTION` header line | Silently accepted (flags set, no error) |
| Comments (`#`, `//`) | Anywhere | Stripped before parsing |
| Blank lines | Anywhere | Ignored |

### 4.2 Forbidden Elements (Trigger Errors)

The following **must not** appear in a language section. The compiler reports `"Should not exist"` for each:

**Type keywords on `*QUESTION` line:**

| Forbidden keyword | Reason |
|-------------------|--------|
| `*SR`, `*MR`, `*OPEN`, `*NUMBER`, `*DATE`, `*TIME` | Type is already set by English section |
| `*GRIDSR`, `*GRIDMR`, `*GRIDOPEN`, `*GRIDNUMBER` | Same |
| `*SLIDERSR`, `*SLIDERMR` | Same |
| `*RANKING`, `*RANKINGMR` | Same |
| `*BARCODE`, `*NPS`, `*PHOTO`, `*GPS`, `*FIFS` | Same |
| `*FORM`, `*MULTIFORM`, `*LOOP` | Same |
| Any type keyword | Language sections are type-agnostic |

**Modifier keywords on `*QUESTION` line:**

| Forbidden keyword |
|-------------------|
| `*RANDOM`, `*ROT`, `*FROT`, `*ROTFIXED`, `*ROTSTART` |
| `*MIN`, `*MAX`, `*NONEXTBTN`, `*NOBACKBTN` |
| `*COLUMN`, `*HORIZONTAL`, `*DUMMY1`, `*DUMMY2` |
| `*DELAY`, `*NOCOLORS` |
| `*IF` (question-level conditional) |
| `*INCLUDE`, `*EXCLUDE` |
| `*USEGRIDLIST` |

**Modifiers on attribute lines:**

All attribute-level modifiers (`*OPEN`, `*NMUL`, `*NOCON`, `*MANDATORY`, `*PICT`, `*VIDEO`, `*LAT`, `*LON`, `*GROUPNAME`, `*GROUPHEAD`, `*COMPVAL`, `*INCLUDE [QId]`, `*EXCLUDE [QId]`, `*EXCEPT`, form sub-types `*SR` / `*MR` / `*ALPHA` / `*NUMBER` / `*DATE` / `*TIME`) trigger `"Should not exist"` warnings when found on attribute lines in a language section.

**Logic directives:**

| Forbidden |
|-----------|
| `*IF [...] *GOTO` |
| `*IF [...] *MSG` |
| `*IF [...] *INCLUDE` / `*EXCLUDE` |
| Standalone `*INCLUDE` / `*EXCLUDE` (logic blocks) |

---

## 5. `*DKCS` in Language Sections

`*DKCS` is the **only modifier** permitted on a language-section `*QUESTION` line (apart from `*END`/`*TERMINATE`).

```
*QUESTION Q5 *DKCS
কোন মতামত নেই          ← this becomes the translated DKCS attribute label
আপনার বয়স কত?
1:১৮-২৪
2:২৫-৩৪
```

When `*DKCS` is present in the language section, the compiler:
1. Reads the **first text line** after the header as the translated DKCS label.
2. Reads subsequent lines as regular attribute translations.
3. Inserts the translated DKCS text as an extra attribute alongside the regular ones.

> **Important:** `*DKCS` must also be present on the English `*QUESTION` line. Adding it only in a language section has no effect on question structure.

---

## 6. `*USELIST` in Language Sections

If the English question uses `*USELIST "ListName"`, you may define a translated version of that list in the language section and reference it the same way:

```
# English section
*LIST "Brands"
1:Coca-Cola
2:Pepsi
3:Sprite

*QUESTION Q1 *SR
Which brand do you prefer?
*USELIST "Brands"

# --- Language section ---
@LANGUAGE "Bengali"

*LIST "Brands"
1:কোকা-কোলা
2:পেপসি
3:স্প্রাইট

*QUESTION Q1
আপনি কোন ব্র্যান্ড পছন্দ করেন?
*USELIST "Brands"
```

The compiler uses a **separate dictionary** (`dicQidVsAttributeListLanX`) for each language's lists, so English and translated lists are stored independently.

---

## 7. Translation Matching Logic

The compiler matches each translated string to the correct English row using:

| Table | Match key |
|-------|-----------|
| `T_Question` | `QId` |
| `T_OptAttribute` | `QId` + `AttributeValue` (the option code, e.g. `1`, `2`, `3`) |
| `T_GridInfo` | `QId` + `AttributeOrder` (sequential row index) |

This means:
- **Option order does not matter** — the code number (`1:`, `2:`, `3:`) is the key, not position in the file.
- **Grid rows are matched by position** — the first translated grid row maps to `AttributeOrder=1`, the second to `AttributeOrder=2`, etc. Do not reorder or omit grid rows.
- **Missing options** in a language section leave the database column NULL for that row; the app falls back to English.

---

## 8. Fallback Behaviour

When a translation is missing (translation column is empty or NULL), the SmartSurvey app displays the English text instead.

Additionally, the compiler's SQL update methods include a **fallback fill query**:

```sql
-- Example for Lang3
SELECT QId, QuestionEnglish, QuestionLang3
FROM T_Question
WHERE QuestionEnglish <> '' AND QuestionLang3 = ''
```

This query identifies any question whose English text was populated but whose translation was not provided. The compiler then copies the English value into the translation column so the mobile app always has something to display.

> **Implication:** You are not required to translate every question. Untranslated questions will simply show in English at runtime.

---

## 9. QId Validation in Language Sections

The compiler validates that:

1. Every `*QUESTION QId` in a language section has a **matching QId** in the English body. Unknown QIds are reported as errors.
2. Within a single language section, **duplicate QIds** are detected and reported.
3. QIds are **case-sensitive** — `Q1` and `q1` are treated as different identifiers.

---

## 10. Complete Annotated Bilingual Script Example

```
# =============================================
# ENGLISH BODY
# =============================================
PROJECT NAME: Brand Tracker
PROJECT CODE: BT001
SCRIPT VERSION: 1.0
SCRIPT NAME: BrandTrackerDB
SCRIPTED BY: JS

*LIST "Brands"
1:Coca-Cola
2:Pepsi
3:Sprite

*GRIDLIST "Agreement"
1:Strongly Agree
2:Agree
3:Neutral
4:Disagree
5:Strongly Disagree

*QUESTION RespName *OPEN
Respondent Name

*QUESTION RespMobile *NUMBER
Respondent Mobile

*QUESTION Centre *SR
Centre
1:Dhaka
2:Chittagong
3:Sylhet

*QUESTION Q1 *SR *ROT *DKCS
DK/CS
Which brand do you prefer most?
*USELIST "Brands"

*IF [Q1=1] *GOTO Q3

*QUESTION Q2 *MR *MAX 2
Which OTHER brands have you tried?
*USELIST "Brands"

*QUESTION Q3 *GRIDSR *USEGRIDLIST "Agreement"
Please rate each brand on the following statement: "This brand is good value."
1:Coca-Cola
2:Pepsi
3:Sprite

*QUESTION Q4 *OPEN
Any other comments?

*QUESTION FIFSInfo *FIFS
Field Interviewer & Supervisor Information

*QUESTION SurveyEnd *END
Thank you for completing the survey!

# =============================================
# LANGUAGE SECTION 1 — Bengali
# (writes to QuestionBengali / AttributeBengali)
# =============================================
@LANGUAGE "Bengali"

*LIST "Brands"
1:কোকা-কোলা
2:পেপসি
3:স্প্রাইট

*GRIDLIST "Agreement"
1:সম্পূর্ণ একমত
2:একমত
3:নিরপেক্ষ
4:দ্বিমত
5:সম্পূর্ণ দ্বিমত

*QUESTION RespName
উত্তরদাতার নাম

*QUESTION RespMobile
উত্তরদাতার মোবাইল

*QUESTION Centre
কেন্দ্র
1:ঢাকা
2:চট্টগ্রাম
3:সিলেট

*QUESTION Q1 *DKCS
জানি না / বলতে পারব না
আপনি কোন ব্র্যান্ড সবচেয়ে বেশি পছন্দ করেন?
*USELIST "Brands"

*QUESTION Q2
আপনি আর কোন ব্র্যান্ড ব্যবহার করে দেখেছেন?
*USELIST "Brands"

*QUESTION Q3
প্রতিটি ব্র্যান্ড সম্পর্কে আপনার মতামত দিন: "এই ব্র্যান্ডটি ভালো মানের।"
1:কোকা-কোলা
2:পেপসি
3:স্প্রাইট

*QUESTION Q4
অন্য কোনো মন্তব্য?

*QUESTION FIFSInfo
মাঠ সাক্ষাৎকারকারী ও সুপারভাইজার তথ্য

*QUESTION SurveyEnd *END
সমীক্ষাটি সম্পন্ন করার জন্য ধন্যবাদ!

# =============================================
# LANGUAGE SECTION 2 — Hindi
# (writes to QuestionLang3 / AttributeLang3)
# =============================================
@LANGUAGE "Hindi"

*LIST "Brands"
1:कोका-कोला
2:पेप्सी
3:स्प्राइट

*GRIDLIST "Agreement"
1:पूरी तरह सहमत
2:सहमत
3:तटस्थ
4:असहमत
5:पूरी तरह असहमत

*QUESTION RespName
उत्तरदाता का नाम

*QUESTION RespMobile
उत्तरदाता का मोबाइल

*QUESTION Centre
केंद्र
1:ढाका
2:चट्टग्राम
3:सिलहट

*QUESTION Q1 *DKCS
पता नहीं / नहीं बता सकता
आप किस ब्रांड को सबसे अधिक पसंद करते हैं?
*USELIST "Brands"

*QUESTION Q2
आपने और कौन से ब्रांड आज़माए हैं?
*USELIST "Brands"

*QUESTION Q3
प्रत्येक ब्रांड के बारे में अपनी राय दें: "यह ब्रांड अच्छे मूल्य का है।"
1:कोका-कोला
2:पेप्सी
3:स्प्राइट

*QUESTION Q4
कोई अन्य टिप्पणी?

*QUESTION FIFSInfo
क्षेत्र साक्षात्कारकर्ता और पर्यवेक्षक जानकारी

*QUESTION SurveyEnd *END
सर्वेक्षण पूरा करने के लिए धन्यवाद!
```

---

## 11. Common Mistakes in Language Sections

| # | Mistake | Error / Symptom | Fix |
|---|---------|-----------------|-----|
| 1 | Adding `*SR`, `*MR`, `*OPEN`, etc. on `*QUESTION` line | `"Should not exist"` error | Remove all type keywords — language sections inherit type from English |
| 2 | Adding `*ROT`, `*MAX`, `*MIN`, `*COLUMN`, etc. | `"Should not exist"` error | Remove all modifier keywords |
| 3 | Including `*IF [...] *GOTO` or `*IF [...] *MSG` blocks | Compiler error | Remove all logic directives |
| 4 | Omitting a question entirely | Translation column stays NULL | App falls back to English — acceptable, not an error |
| 5 | Using a QId that doesn't exist in English | Error: unknown QId | Check spelling/case against English QId list |
| 6 | Duplicating a QId within one language section | Duplicate QId error | Each QId must appear only once per `@LANGUAGE` block |
| 7 | Reordering grid rows | Grid translations silently misaligned | Grid matching is by `AttributeOrder`, not option code — keep same row order as English |
| 8 | Adding modifiers to attribute lines (`1:Label *OPEN`) | `"Should not exist"` warning | Attribute lines in language sections contain label text only |
| 9 | Defining more than 9 `@LANGUAGE` blocks | Beyond Lang10 — no DB column exists | Maximum 9 language sections |
| 10 | Placing `@LANGUAGE` marker inside the English body | Compiler ends English parsing prematurely | Always place all `@LANGUAGE` blocks after `*QUESTION SurveyEnd *END` |
| 11 | Forgetting `*DKCS` in a language section when it was in English | DKCS attribute shows in English only | Mirror `*DKCS` in the language section and provide the translated label as the first line |
| 12 | Using a different list name in `*USELIST` than in English | List not found; attributes blank | List name in `*USELIST` must exactly match the name used in the English `*USELIST` |

---

## 12. Quick-Reference Checklist

Before submitting a multi-language script:

- [ ] All `@LANGUAGE` blocks appear **after** `SurveyEnd` / end of English body
- [ ] Each `*QUESTION QId` in language sections matches an English QId exactly (case-sensitive)
- [ ] No type keywords (`*SR`, `*MR`, `*OPEN`, etc.) on any `*QUESTION` line in language sections
- [ ] No modifier keywords (`*ROT`, `*MAX`, `*COLUMN`, etc.) in language sections
- [ ] No logic directives (`*IF`, `*GOTO`, `*INCLUDE`, `*EXCLUDE`) in language sections
- [ ] `*DKCS` mirrored in each language section for every question that uses it in English
- [ ] Grid questions: translated rows in the **same order** as English rows
- [ ] Named lists (`*LIST`) re-defined in each language section if used via `*USELIST`
- [ ] No QId appears more than once within a single `@LANGUAGE` block
- [ ] Total `@LANGUAGE` blocks ≤ 9
