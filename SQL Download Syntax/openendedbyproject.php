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
   $query = "SELECT open_endeds_".$projectCode.".`id` , open_endeds_".$projectCode.".`interview_info_id` , open_endeds_".$projectCode.".`project_id` , open_endeds_".$projectCode.".`respondent_id` , open_endeds_".$projectCode.".`q_id` , open_endeds_".$projectCode.".`attribute_value` , open_endeds_".$projectCode.".`response` , open_endeds_".$projectCode.".`response_type` , open_endeds_".$projectCode.".`created_at` , open_endeds_".$projectCode.".`deleted_at`
			FROM  open_endeds_".$projectCode."  INNER JOIN interview_infos_".$projectCode." ON open_endeds_".$projectCode.".`interview_info_id` = interview_infos_".$projectCode.".`id`
          	WHERE  interview_infos_".$projectCode.".project_id=".$projectCode." AND interview_infos_".$projectCode.".survey_start_at BETWEEN '".$startDate."' AND '".$endDate."' AND interview_infos_".$projectCode.".intv_type='".$interviewType."' AND interview_infos_".$projectCode.".deleted_at IS NULL";
          }else{
            $query = "SELECT open_endeds_".$projectCode.".`id` , open_endeds_".$projectCode.".`interview_info_id` , open_endeds_".$projectCode.".`project_id` , open_endeds_".$projectCode.".`respondent_id` , open_endeds_".$projectCode.".`q_id` , open_endeds_".$projectCode.".`attribute_value` , open_endeds_".$projectCode.".`response` , open_endeds_".$projectCode.".`response_type` , open_endeds_".$projectCode.".`created_at` , open_endeds_".$projectCode.".`deleted_at`
      FROM  open_endeds_".$projectCode." INNER JOIN interview_infos_".$projectCode." ON open_endeds_".$projectCode.".`interview_info_id` = interview_infos_".$projectCode.".`id`
            WHERE  interview_infos_".$projectCode.".project_id=".$projectCode." AND interview_infos_".$projectCode.".created_at BETWEEN '".$startDate."' AND '".$endDate."' AND interview_infos_".$projectCode.".intv_type='".$interviewType."' AND interview_infos_".$projectCode.".deleted_at IS NULL";
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