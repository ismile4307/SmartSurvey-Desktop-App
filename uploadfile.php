<?php

// ============================================================
//  uploadfile.php
//  Receives an SQLite .db script file, saves it, then syncs
//  all script tables into the MySQL database.
// ============================================================

// ── Database configuration ───────────────────────────────────
define('MYSQL_HOST', 'localhost');
define('MYSQL_DB',   'survfiqz_smartsurvey');
define('MYSQL_USER', 'survfiqz_ismile');
define('MYSQL_PASS', 'Arnisha@4307#');
// ─────────────────────────────────────────────────────────────

if (!isset($_FILES['file']['tmp_name'])) {
    echo "No file uploaded.";
    exit;
}

$name        = basename($_FILES['file']['name']);
$destination = "../scripts/" . $name;

if (!move_uploaded_file($_FILES['file']['tmp_name'], $destination)) {
    echo "Failed to save uploaded file.";
    exit;
}

// ── MySQL connection ─────────────────────────────────────────
try {
    $mysql = new PDO(
        "mysql:host=" . MYSQL_HOST . ";dbname=" . MYSQL_DB . ";charset=utf8mb4",
        MYSQL_USER,
        MYSQL_PASS,
        [PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION]
    );
} catch (PDOException $e) {
    echo "MySQL connection failed: " . $e->getMessage();
    exit;
}

// ── SQLite connection ────────────────────────────────────────
try {
    $sqlite = new PDO("sqlite:" . $destination);
    $sqlite->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
} catch (PDOException $e) {
    echo "SQLite connection failed: " . $e->getMessage();
    exit;
}

// ── Read project ID from SQLite ──────────────────────────────
$projRow = $sqlite->query("SELECT ProjectId FROM T_ProjectInfo LIMIT 1")->fetch(PDO::FETCH_ASSOC);
if (!$projRow) {
    echo "Cannot read project ID from uploaded database.";
    exit;
}
$projectId = (int)$projRow['ProjectId'];

// ── Sync: delete then re-insert inside a transaction ─────────
$mysql->beginTransaction();

try {

    // Delete existing data for this project from all tables
    $tablesToClear = [
        'questions',
        'attributes',
        'filter_attributes',
        'logic_tables',
        'logic_autos',
        'language_masters',
        'grid_infos',
        'panel_data',
    ];
    foreach ($tablesToClear as $table) {
        $mysql->exec("DELETE FROM `{$table}` WHERE project_id = {$projectId}");
    }

    // ── 1. T_Question → questions ────────────────────────────
    $rows = $sqlite->query("SELECT * FROM T_Question WHERE QId != ''")->fetchAll(PDO::FETCH_NUM);
    $stmt = $mysql->prepare("
        INSERT INTO questions (
            project_id, qid, question_english, question_bengali,
            attribute_id, comments, qtype,
            no_of_response_min, no_of_response_max,
            has_auto_response, has_random_attrib, number_of_column,
            show_in_report, has_random_qntr, has_message_logic,
            written_oe_in_paper, force_to_take_oe, has_media_path,
            display_back_button, display_next_button, display_jump_button,
            resume_qntr_jump, silent_recording, file_path,
            order_tag, order_tag1, order_tag2, order_tag3, order_tag4, order_tag5,
            question_lang3, question_lang4, question_lang5, question_lang6,
            question_lang7, question_lang8, question_lang9, question_lang10
        ) VALUES (
            ?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?
        )
    ");
    foreach ($rows as $r) {
        $stmt->execute([
            $r[0],  $r[1],  $r[2],  $r[3],  $r[4],  $r[5],  $r[6],  $r[7],
            $r[8],  $r[9],  $r[10], $r[11], $r[12], $r[13], $r[14], $r[15],
            $r[16], $r[17], $r[18], $r[19], $r[20], $r[21], $r[22], $r[23],
            $r[24], $r[25], $r[26], $r[27], $r[28], $r[29], $r[30], $r[31],
            $r[32], $r[33], $r[34], $r[35], $r[36], $r[37],
        ]);
    }

    // ── 2. T_OptAttribute → attributes ──────────────────────
    $rows = $sqlite->query("SELECT * FROM T_OptAttribute WHERE QId != ''")->fetchAll(PDO::FETCH_NUM);
    $stmt = $mysql->prepare("
        INSERT INTO attributes (
            project_id, qid, attribute_english, attribute_bengali,
            attribute_value, attribute_order, take_openended, is_exclusive,
            link_id1, link_id2, min_value, max_value, force_and_msg_opt,
            group_name, filter_qid, filter_type, excep_value, comments,
            attribute_lang3, attribute_lang4, attribute_lang5, attribute_lang6,
            attribute_lang7, attribute_lang8, attribute_lang9, attribute_lang10
        ) VALUES (
            ?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?
        )
    ");
    foreach ($rows as $r) {
        $stmt->execute([
            $r[0],  $r[1],  $r[2],  $r[3],  $r[4],  $r[5],  $r[6],  $r[7],
            $r[8],  $r[9],  $r[10], $r[11], $r[12], $r[13], $r[14], $r[15],
            $r[16], $r[17], $r[18], $r[19], $r[20], $r[21], $r[22], $r[23],
            $r[24], $r[25],
        ]);
    }

    // ── 3. T_OptAttrbFilter → filter_attributes ──────────────
    $rows = $sqlite->query("SELECT * FROM T_OptAttrbFilter WHERE QId != ''")->fetchAll(PDO::FETCH_NUM);
    $stmt = $mysql->prepare("
        INSERT INTO filter_attributes (
            project_id, attrib_filter_id, qid, inherited_qId,
            filter_type, exceptional_value, label_taken_from
        ) VALUES (?,?,?,?,?,?,?)
    ");
    foreach ($rows as $r) {
        $stmt->execute([$r[0], $r[1], $r[2], $r[3], $r[4], $r[5], $r[6]]);
    }

    // ── 4. T_LogicTable → logic_tables ──────────────────────
    // Column index 1 (SQLite internal id) is skipped; logic_id is a
    // sequential counter matching the C# desktop app behaviour.
    $rows   = $sqlite->query("SELECT * FROM T_LogicTable WHERE QId != ''")->fetchAll(PDO::FETCH_NUM);
    $stmt   = $mysql->prepare("
        INSERT INTO logic_tables (
            project_id, logic_id, qid, logic_type_id,
            if_condition, then_value, else_value
        ) VALUES (?,?,?,?,?,?,?)
    ");
    $logicId = 1;
    foreach ($rows as $r) {
        $stmt->execute([$r[0], $logicId++, $r[2], $r[3], $r[4], $r[5], $r[6]]);
    }

    // ── 5. T_LogicAuto → logic_autos ────────────────────────
    $rows   = $sqlite->query("SELECT * FROM T_LogicAuto WHERE QId != ''")->fetchAll(PDO::FETCH_NUM);
    $stmt   = $mysql->prepare("
        INSERT INTO logic_autos (
            project_id, logic_id, qid, logic_type_id,
            if_condition, then_value, else_value
        ) VALUES (?,?,?,?,?,?,?)
    ");
    $logicId = 1;
    foreach ($rows as $r) {
        $stmt->execute([$r[0], $logicId++, $r[2], $r[3], $r[4], $r[5], $r[6]]);
    }

    // ── 6. T_LanguageMaster → language_masters ───────────────
    // Indices 3 and 4 are internal SQLite columns not mapped to MySQL.
    $rows = $sqlite->query("SELECT * FROM T_LanguageMaster WHERE Status = '1'")->fetchAll(PDO::FETCH_NUM);
    $stmt = $mysql->prepare("
        INSERT INTO language_masters (
            project_id, language_id, language_name, status, display_order
        ) VALUES (?,?,?,?,?)
    ");
    foreach ($rows as $r) {
        $stmt->execute([$r[0], $r[1], $r[2], $r[5], $r[6]]);
    }

    // ── 7. T_GridInfo → grid_infos ──────────────────────────
    // Rows where QId (index 1) is empty are skipped, matching C# behaviour.
    $rows = $sqlite->query("SELECT * FROM T_GridInfo WHERE QId != ''")->fetchAll(PDO::FETCH_NUM);
    $stmt = $mysql->prepare("
        INSERT INTO grid_infos (
            project_id, qid, attribute_english, attribute_bengali,
            attribute_value, attribute_order, take_openended, is_exclusive,
            min_value, max_value, force_and_msg_opt, comments,
            attribute_lang3, attribute_lang4, attribute_lang5, attribute_lang6,
            attribute_lang7, attribute_lang8, attribute_lang9, attribute_lang10
        ) VALUES (
            ?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?
        )
    ");
    foreach ($rows as $r) {
        if ($r[1] === '') continue;
        $stmt->execute([
            $r[0],  $r[1],  $r[2],  $r[3],  $r[4],  $r[5],  $r[6],  $r[7],
            $r[8],  $r[9],  $r[10], $r[11], $r[12], $r[13], $r[14], $r[15],
            $r[16], $r[17], $r[18], $r[19],
        ]);
    }

    // ── 8. T_PanelData → panel_data ─────────────────────────
    // Column names are read dynamically from the SQLite table and assumed
    // to match the MySQL panel_data column names exactly.
    try {
        $panelRows = $sqlite->query("SELECT * FROM T_PanelData")->fetchAll(PDO::FETCH_ASSOC);
        if (!empty($panelRows)) {
            $cols         = array_keys($panelRows[0]);
            $colList      = implode(', ', array_map(fn($c) => "`{$c}`", $cols));
            $placeholders = implode(', ', array_fill(0, count($cols), '?'));
            $stmt         = $mysql->prepare("INSERT INTO panel_data ({$colList}) VALUES ({$placeholders})");
            foreach ($panelRows as $r) {
                $stmt->execute(array_values($r));
            }
        }
    } catch (PDOException $e) {
        // T_PanelData may not exist in all script files — skip silently
    }

    $mysql->commit();
    echo "Script uploaded successfully..";

} catch (Exception $e) {
    $mysql->rollBack();
    echo "Upload failed: " . $e->getMessage();
}
?>
