<?php
require __DIR__ . '/deskapi_common.php';

$connection = desk_connect();
$p = desk_params($connection);

$o  = 'open_endeds_' . $p['projectCode'];
$ii = 'interview_infos_' . $p['projectCode'];

$query = "SELECT $o.`id`, $o.`interview_info_id`, $o.`project_id`, $o.`respondent_id`, $o.`q_id`, $o.`attribute_value`,
                 $o.`response`, $o.`response_type`, $o.`created_at`, $o.`deleted_at`
          FROM $o INNER JOIN $ii ON $o.`interview_info_id` = $ii.`id`
          WHERE " . desk_interview_filter($p);

if ($p['keyset']) {
    $query .= " AND $o.`id` > " . $p['lastId'] . " ORDER BY $o.`id` LIMIT " . $p['pageSize'];
} else {
    $query .= " ORDER BY $o.`id`";   // older clients: everything in one response, as before
}

desk_stream_json($connection, $query);
mysqli_close($connection);
