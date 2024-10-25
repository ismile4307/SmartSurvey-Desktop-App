<?php
    //open connection to mysql db
    $connection = mysqli_connect("localhost","chromsha_surveyhive","Arnisha@4307#","chromsha_surveyHive") or die("Error " . mysqli_error($connection));

    //fetch table rows from mysql db
   $interviewerId = $_POST['interviewerid'];
   $password = $_POST['password'];

   $query = "SELECT interviewer_id FROM `interviewers` WHERE interviewer_id='".$interviewerId."' AND password='".$password."'";
   
   echo $query;
   //$query = "SELECT interviewer_id FROM `interviewers` WHERE interviewer_id='test' AND password='test'";

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