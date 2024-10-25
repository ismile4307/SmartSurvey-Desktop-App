<?php
    //open connection to mysql db
    $connection = mysqli_connect("localhost","survfiqz_ismile","Arnisha@4307#","survfiqz_capi") or die("Error " . mysqli_error($connection));

    //fetch table rows from mysql db
   $startDate = $_POST['startDate'] . " 00:00:00";
   $endDate = $_POST['endDate'] . " 23:59:59";
   $dateType= $_POST['dateType'];
   $projectCode= $_POST['projectCode'];
   $interviewType=$_POST['interviewType'];

   IF ($dateType=="1"){
   $query = "SELECT open_endeds.`id` , open_endeds.`interview_info_id` , open_endeds.`project_id` , open_endeds.`respondent_id` , open_endeds.`q_id` , open_endeds.`attribute_value` , open_endeds.`response` , open_endeds.`response_type` , open_endeds.`created_at` , open_endeds.`deleted_at`
			FROM  `open_endeds` INNER JOIN interview_infos ON open_endeds.`interview_info_id` = interview_infos.`id`
          	WHERE  interview_infos.project_id=".$projectCode." AND interview_infos.survey_start_at BETWEEN '".$startDate."' AND '".$endDate."' AND interview_infos.intv_type='".$interviewType."' AND interview_infos.deleted_at IS NULL";
          }else{
            $query = "SELECT open_endeds.`id` , open_endeds.`interview_info_id` , open_endeds.`project_id` , open_endeds.`respondent_id` , open_endeds.`q_id` , open_endeds.`attribute_value` , open_endeds.`response` , open_endeds.`response_type` , open_endeds.`created_at` , open_endeds.`deleted_at`
      FROM  `open_endeds` INNER JOIN interview_infos ON open_endeds.`interview_info_id` = interview_infos.`id`
            WHERE  interview_infos.project_id=".$projectCode." AND interview_infos.created_at BETWEEN '".$startDate."' AND '".$endDate."' AND interview_infos.intv_type='".$interviewType."' AND interview_infos.deleted_at IS NULL";
          }

   $result = mysqli_query($connection, $query) or die("Error in Selecting " . mysqli_error($connection));

    //create an array
    $emparray = array();
    while($row =mysqli_fetch_assoc($result))
    {
        $emparray[] = $row;
    }
    echo json_encode($emparray);

    //close the db connection
    mysqli_close($connection);
?>