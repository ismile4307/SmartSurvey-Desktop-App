<?php
    //open connection to mysql db
    $connection = mysqli_connect("localhost","survfiqz_ismile","Arnisha@4307#","survfiqz_capiapp") or die("Error " . mysqli_error($connection));

    //fetch table rows from mysql db
   if (isset($_POST['interviewer_id'])) 
    {
        $interviewerId = $_POST['interviewer_id'];
    }

   if (isset($_POST['password'])) 
    {
        $password = $_POST['password'];
    }
    
    $query="SELECT projects.id, projects.project_code, projects.project_name, projects.script_version, projects.media_version, projects.client_name, projects.start_date, projects.database_name  FROM `projects` INNER JOIN project_users ON projects.id = project_users.Project_id INNER JOIN interviewers ON project_users.interviewer_id = interviewers.id WHERE interviewers.interviewer_id='".$interviewerId."' AND interviewers.password='".$password."' AND project_users.Is_assigned=1";

   //echo $query;
   
   $result = mysqli_query($connection, $query) or die("Error in Selecting " . mysqli_error($connection));

    //create an array
    $emparray = array();
    while($row =mysqli_fetch_assoc($result))
    {
        $emparray[] = $row;
    }

    
    if (empty($emparray))
        echo json_encode(array('HasError'=>'TRUE','Message'=>'Login Failed...','loginData'=>$emparray));
    else
        echo json_encode(array('HasError'=>'FALSE','Message'=>'Login Successful...','loginData'=>$emparray));

    //echo json_encode($emparray);


    //close the db connection
    mysqli_close($connection);
?>