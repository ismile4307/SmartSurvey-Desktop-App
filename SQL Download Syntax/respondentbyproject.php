<?php
    //open connection to mysql db
    $connection = mysqli_connect("localhost","survfiqz_ismile","Arnisha@4307#","survfiqz_capi") or die("Error " . mysqli_error($connection));

    //fetch table rows from mysql db
   $startDate = $_POST['startDate'] . " 00:00:00";
   $endDate = $_POST['endDate'] . " 23:59:59";
   $dateType= $_POST['dateType'];
   $projectCode= $_POST['projectCode'];
   $interviewType=$_POST['interviewType'];

   IF ($projectCode=="12642")
   {
      $sql = "UPDATE `answers` SET `resp_order`=1 WHERE `resp_order`=0 AND project_id=".$projectCode;

      $result1 = mysqli_query($connection, $sql);

      // if ($connection->query($sql) === TRUE) {
      //     //echo "Record updated successfully";
      // } else {
      //     //echo "Error updating record: " . $conn->error;
      // }
   }

   IF ($dateType=="1"){
   $query = "SELECT * FROM interview_infos_".$projectCode." WHERE project_id=".$projectCode." AND survey_start_at BETWEEN '" . $startDate . "' AND '" . $endDate . "' AND intv_type='".$interviewType."' AND deleted_at IS NULL";
  }else{
    $query = "SELECT * FROM interview_infos_".$projectCode." WHERE project_id=".$projectCode." AND created_at BETWEEN '" . $startDate . "' AND '" . $endDate . "' AND intv_type='".$interviewType."' AND deleted_at IS NULL";
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