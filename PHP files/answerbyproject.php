<?php
require __DIR__ . '/deskapi_common.php';

$connection = desk_connect();
$p = desk_params($connection);

$a  = 'answers_' . $p['projectCode'];
$ii = 'interview_infos_' . $p['projectCode'];

$query = "SELECT $a.`id`, $a.`interview_info_id`, $a.`project_id`, $a.`respondent_id`, $a.`q_id`, $a.`response`,
                 $a.`responded_at`, $a.`q_elapsed_time`, $a.`q_order`, $a.`resp_order`, $a.`created_at`, $a.`deleted_at`
          FROM $a INNER JOIN $ii ON $a.`interview_info_id` = $ii.`id`
          WHERE " . desk_interview_filter($p);

if ($p['keyset']) {
    $query .= " AND $a.`id` > " . $p['lastId'] . " ORDER BY $a.`id` LIMIT " . $p['pageSize'];
} else {
    // Older clients page with myOffset (10,000 rows per page); ORDER BY makes the pages stable.
    $query .= " ORDER BY $a.`id` LIMIT 10000 OFFSET " . $p['offset'];
}

desk_stream_json($connection, $query);
mysqli_close($connection);
