# Part 8 — `*REPEAT` Patterns & Curly-Brace References

> **Source:** `DBI Scripting\Forms\Scripting\FrmBuildScript.xaml.cs`
> **Relevant methods:** `BuildRepeatIterationList`, `ExpandRepeatBlockEnglish`, `ExpandRepeatBlockLanguage`, `CheckCurlyReferences`

---

## Overview

SmartSurvey provides two mechanisms for generating questions from a template:

| Mechanism | Keyword | What it generates |
|-----------|---------|-------------------|
| **Block REPEAT** | `*REPEAT [source]` … `*ENDREPEAT` | One full copy of every question/list/logic inside the block per iteration value |
| **Inline REPEAT** | `*QUESTION QId *REPEAT [SourceQId]` | One copy of a single question per attribute of the source question |

Both use `?R` as the substitution placeholder. Curly-brace references (`{QId}` / `{QId.N}`) let question text display a previously-recorded answer — a separate but related feature.

---

## 1. Block REPEAT

### 1.1 Syntax

```
*REPEAT [source]
  ... template lines ...
*ENDREPEAT
```

| Element | Detail |
|---------|--------|
| `*REPEAT` | Must be the first token on the line |
| `[source]` | Square brackets are required; contains a numeric range or a QId |
| Template lines | Any valid script lines; use `?R` where the iteration value should appear |
| `*ENDREPEAT` | Marks the end of the block; must be present or the compiler reports an error and skips to the next `*ENDREPEAT` |

### 1.2 Source Types

#### Numeric Range

```
*REPEAT [1 TO 5]
```

- Iterates with values `1`, `2`, `3`, `4`, `5`
- Start must be **less than** end (start ≥ end is an error)
- Values are plain integers; `?R` receives `"1"`, `"2"`, etc.

#### QId Source

```
*REPEAT [BrandList]
```

- Uses the attribute codes (`AttributeValue`) of the already-parsed question `BrandList` as the iteration list
- The source QId must have been defined **before** this `*REPEAT` block in the script
- Iteration stops early if any attribute has `AttributeEnglish` containing `"None"` (sentinel value)
- If the source QId is not found or has no attributes, an error is reported and the block is skipped

### 1.3 The `?R` Placeholder

`?R` is a literal text token that is replaced by the current iteration value before the line is parsed. It can appear:

- In a `*QUESTION` line (most commonly in the QId): `*QUESTION Brand?R *SR`
- In question text: `Tell us about brand {Brand?R}`
- In attribute labels: `1:Brand ?R`
- In `*IF` conditions: `*IF [Brand?R=1] *GOTO NextSection`
- In list names: `*LIST "Brand?RList"` *(rare)*

**`?R` is case-sensitive.** `?r` is not substituted.

### 1.4 What Can Appear Inside a Block REPEAT

The following are supported inside a block REPEAT in the **English body**:

| Element | Notes |
|---------|-------|
| `*LIST "name"` | Defines a named option list for use within the block |
| `*GRIDLIST "name"` | Defines a named grid column list |
| `*IF [...] *GOTO QId` | GOTO logic referencing a question outside or inside the block |
| `*IF [...] *MSG "text"` | Message logic |
| `*IF [...] *INCLUDE QId [...]` | Auto-response include logic |
| `*IF [...] *EXCLUDE QId [...]` | Auto-response exclude logic |
| `*INCLUDE QId [...]` | Standalone include logic |
| `*EXCLUDE QId [...]` | Standalone exclude logic |
| `*STARTREC "path"` | Begin silent recording section |
| `*ENDREC` | End silent recording section |
| `*QUESTION QId …` | Full question definition with any modifiers and attributes |

**Inside a language-section REPEAT block**, only these are supported (same restriction as language sections generally):

| Element | Supported |
|---------|-----------|
| `*LIST "name"` | Yes |
| `*GRIDLIST "name"` | Yes |
| `*QUESTION QId` (text + attributes only) | Yes |
| Logic directives, type keywords, modifiers | No |

### 1.5 Two-Pass Expansion (English only)

The English block expander uses two passes to handle cross-iteration `*IF` references:

**Pass 1 — Pre-registration:**
All QIds that will be generated (e.g. `Brand1`, `Brand2`, `Brand3`) are registered in the duplicate-check list **before** any questions are parsed. This lets a condition like `*IF [Brand1=1] *GOTO Brand3` inside the block resolve correctly even though `Brand3` hasn't been parsed yet.

**Pass 2 — Expansion:**
For each iteration value, every `?R` in the buffer is substituted, then the resulting lines are fed through the standard parsers (`prepareList`, `prepareGridList`, `prepareIf`, `prepareIncludeExclude`, `prepareQuestion`). Results are merged into the global question/logic/attribute collections.

Language section blocks use only a single pass (no logic to pre-register).

### 1.6 Complete Block REPEAT Example

**Goal:** Ask the same battery of questions for 3 brands.

```
# Define brands as a question so the repeat block can source from it
*QUESTION Q_Brands *SR
Which brand are you evaluating?
1:Coca-Cola
2:Pepsi
3:Sprite

# --- Block repeat over brand codes 1, 2, 3 ---
*REPEAT [Q_Brands]

*QUESTION Brand?R_Aware *SR
Are you aware of brand {Q_Brands.?R}?
1:Yes
2:No

*IF [Brand?R_Aware=2] *GOTO Brand?R_End

*QUESTION Brand?R_Rate *SR
How would you rate brand {Q_Brands.?R}?
1:Excellent
2:Good
3:Fair
4:Poor

*QUESTION Brand?R_End *INFO
(End of brand ?R section)

*ENDREPEAT
```

**What the compiler generates (expanded):**

```
*QUESTION Brand1_Aware *SR   ← iteration 1
*QUESTION Brand1_Rate  *SR
*QUESTION Brand1_End   *INFO
*QUESTION Brand2_Aware *SR   ← iteration 2
*QUESTION Brand2_Rate  *SR
*QUESTION Brand2_End   *INFO
*QUESTION Brand3_Aware *SR   ← iteration 3
*QUESTION Brand3_Rate  *SR
*QUESTION Brand3_End   *INFO
```

And the GOTO rule `*IF [Brand1_Aware=2] *GOTO Brand1_End` etc. is registered for each iteration.

---

## 2. Inline REPEAT

### 2.1 Syntax

```
*QUESTION QId *REPEAT [SourceQId]
Question text
```

The inline form is a modifier on a `*QUESTION` line. It does **not** use `*ENDREPEAT` and does **not** use `?R`.

| Element | Detail |
|---------|--------|
| `QId` | The **same** QId is reused for every generated question |
| `*REPEAT [SourceQId]` | References a question already defined earlier in the script |
| `SourceQId` | Must be an existing QId (not a range, not a list name) |

### 2.2 How It Works

For each attribute of `SourceQId` (in order, stopping at `"None"` sentinel):

1. A new `AttributeMain` is created holding the source attribute's `AttributeEnglish` and `AttributeValue`.
2. The entire `*QUESTION` block (from this line to the next `*QUESTION`/`*LIST`/`*GRIDLIST`) is parsed via `prepareQuestion` with that `AttributeMain` injected as the single attribute.
3. One `Question` row is inserted into `T_Question` per source attribute.

**Effect:** The QId is the same across all generated rows but each question gets a different single attribute — effectively turning one question template into N questions-with-one-attribute each.

> This is an older mechanism. The block REPEAT (`*REPEAT [source]` … `*ENDREPEAT`) is more flexible and is preferred for new scripts.

### 2.3 Inline REPEAT Example

```
*QUESTION Q_Brands *SR
Which brand?
1:Coca-Cola
2:Pepsi
3:Sprite

*QUESTION BrandRating *REPEAT [Q_Brands]
Rate this brand.
```

The compiler generates 3 instances of `BrandRating`, each with one attribute taken from `Q_Brands`:
- `BrandRating` with attribute `1:Coca-Cola`
- `BrandRating` with attribute `2:Pepsi`
- `BrandRating` with attribute `3:Sprite`

---

## 3. Numeric Range vs QId Source — When to Use Each

| Situation | Recommended source |
|-----------|--------------------|
| Fixed number of iterations (e.g. always 5 rounds) | `*REPEAT [1 TO 5]` |
| Iterations driven by a dynamic option list | `*REPEAT [Q_SourceQId]` |
| Question text needs to display the brand name | QId source + `{Q_Brands.?R}` curly reference |
| Question text only needs a number suffix | Numeric range is simpler |
| You want each question to carry the brand as its attribute | Inline REPEAT or block REPEAT with a single-attribute question |

---

## 4. Curly-Brace References `{QId}` and `{QId.N}`

### 4.1 Syntax

Curly-brace references can appear anywhere in question text or attribute labels:

| Syntax | Meaning |
|--------|---------|
| `{QId}` | Display the answer recorded for question `QId` |
| `{QId.N}` | Display the label of attribute `N` from question `QId` |

**Regex matched by the compiler:** `\{([^}]+)\}` — any `{...}` in a text line.

### 4.2 Validation at Compile Time

`CheckCurlyReferences` is called on every question text line in all sections (English + all language sections):

| Case | Compiler action |
|------|----------------|
| `{QId}` — QId found (exact case match) | OK — no message |
| `{QId}` — QId found but case differs | **WARNING:** `{QId} — QId 'q1' case mismatch, defined as 'Q1'` |
| `{QId}` — QId not found at all | **ERROR:** `{QId} — QId 'Q99' is not defined` |

### 4.3 Where the Reference is Checked

| Section | Called from |
|---------|-------------|
| English question text | Inside `prepareQuestion`, after the question text line is read |
| Language section question text | Inside `prepareQuestionForLanguage`, same position |

> Curly-brace references are a **runtime** feature — the app substitutes the live recorded value. The compiler only validates that the QId exists; it does not validate `N` in `{QId.N}`.

### 4.4 Example Usage

```
*QUESTION Q1 *OPEN
What is your name?

*QUESTION Q2 *SR
Thank you, {Q1}. Which city do you live in?
1:Dhaka
2:Chittagong
3:Sylhet

*QUESTION Q3 *SR
You said you live in {Q2.?R} — is that correct?   # inside a *REPEAT block
1:Yes
2:No
```

---

## 5. REPEAT in Language Sections

Both block and inline REPEAT work in `@LANGUAGE` sections. The source and iteration values are the same as the English body — the language section REPEAT block provides translated text for the same expanded questions.

### Rules for REPEAT in language sections

- `?R` substitution works identically — the same iteration values are used
- `*LIST` and `*GRIDLIST` inside the language section REPEAT block provide translated labels
- `*QUESTION QId` (with `?R` in the QId) provides the translated question text
- No type keywords, modifiers, or logic directives — same restrictions as any language section
- The language section REPEAT block does **not** need to list every iteration's question — missing ones fall back to English

### Language section REPEAT example

```
@LANGUAGE "Bengali"

*REPEAT [Q_Brands]

*QUESTION Brand?R_Aware
আপনি কি {Q_Brands.?R} ব্র্যান্ড সম্পর্কে সচেতন?
1:হ্যাঁ
2:না

*QUESTION Brand?R_Rate
{Q_Brands.?R} ব্র্যান্ডকে কীভাবে রেট দেবেন?
1:চমৎকার
2:ভালো
3:মধ্যম
4:খারাপ

*ENDREPEAT
```

---

## 6. Error Reference

| Error message | Cause | Fix |
|---------------|-------|-----|
| `*REPEAT syntax invalid — missing [source]` | No `[...]` on `*REPEAT` line | Add `[source]` in square brackets |
| `*REPEAT block not closed with *ENDREPEAT` | EOF or next section reached before `*ENDREPEAT` | Add `*ENDREPEAT` after the last template line |
| `*REPEAT range invalid: start must be less than end` | `[5 TO 3]` or `[4 TO 4]` | Swap or widen the range |
| `*REPEAT source QId 'X' not found` | Source QId doesn't exist yet | Move `*REPEAT` after the source question, or fix the QId name |
| `*REPEAT source QId 'X' has no attributes` | Source question was parsed but has no options | Add options to the source question |
| `*REPEAT references unknown QId 'X'` | Inline `*REPEAT [X]` on `*QUESTION` line — X not found | Check spelling; source must appear before the inline repeat question |
| Duplicate QId error inside block | A generated QId (e.g. `Brand1`) collides with an existing question | Rename the outer question or use a different `?R` position in the QId |
| `{QId} — QId 'X' is not defined` | Curly reference to an unknown QId | Fix the QId in `{...}` or define the referenced question earlier |
| `{QId} — QId 'x' case mismatch, defined as 'X'` | Curly reference with wrong case | Correct the case to match the defined QId exactly |

---

## 7. Common Patterns Cheat-Sheet

### Pattern A — Brand battery with numeric range

```
*REPEAT [1 TO 4]
*QUESTION BrandAware?R *SR
Are you aware of Brand ?R?
1:Yes
2:No

*QUESTION BrandRate?R *SR *IF [BrandAware?R=1]
Rate Brand ?R.
1:Poor
2:Average
3:Good
4:Excellent

*ENDREPEAT
```

### Pattern B — Battery sourced from a question's attribute list

```
*QUESTION Q_Brands *MR *MAX 5
Which brands have you heard of?
1:Brand A
2:Brand B
3:Brand C
4:Brand D

*REPEAT [Q_Brands]
*QUESTION Aware?R *SR
You mentioned Brand ?R — how often do you use it?
1:Daily
2:Weekly
3:Monthly
4:Never
*ENDREPEAT
```

### Pattern C — Grid question with `?R` as row label source

```
*GRIDLIST "Scale"
1:Strongly Agree
2:Agree
3:Neutral
4:Disagree
5:Strongly Disagree

*REPEAT [1 TO 3]
*QUESTION BrandOpinion?R *GRIDSR *USEGRIDLIST "Scale"
Rate the following statements about Brand ?R.
1:Good value for money
2:High quality
3:Widely available
*ENDREPEAT
```

### Pattern D — Inline REPEAT (older style)

```
*QUESTION Q_Products *SR
Choose a product.
1:Shampoo
2:Conditioner
3:Body Wash

*QUESTION ProdRating *REPEAT [Q_Products]
How satisfied are you with this product?
```

### Pattern E — Curly reference in repeated question text

```
*QUESTION Q_Brands *SR
Which brand do you use most?
1:Coca-Cola
2:Pepsi
3:Sprite

*REPEAT [Q_Brands]
*QUESTION BrandDetail?R *OPEN
You chose {Q_Brands.?R}. What do you like most about it?
*ENDREPEAT
```
