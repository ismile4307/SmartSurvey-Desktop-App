<?php
require __DIR__ . '/deskapi_common.php';

$connection = desk_connect();
$p = desk_params($connection);

$ii = 'interview_infos_' . $p['projectCode'];

// Data fix for project 12642 kept from the previous version; now runs once per download, not per page.
if ($p['projectCode'] === '12642' && $p['lastId'] === 0) {
    mysqli_query($connection, "UPDATE `answers` SET `resp_order`=1 WHERE `resp_order`=0 AND project_id=12642");
}

$query = "SELECT * FROM $ii WHERE " . desk_interview_filter($p);

if ($p['keyset']) {
    $query .= " AND `id` > " . $p['lastId'] . " ORDER BY `id` LIMIT " . $p['pageSize'];
} else {
    $query .= " ORDER BY `id`";   // older clients: everything in one response, as before
}

desk_stream_json($connection, $query);
mysqli_close($connection);
