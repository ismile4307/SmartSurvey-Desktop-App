<?php

if(isset($_FILES['file']['tmp_name'])) {
    $filepath = $_FILES["file"]["tmp_name"];
    $name= basename($_FILES["file"]["name"]);
    // if(file_exists($name)) {
    //     unlink($name);
    // }

    move_uploaded_file($filepath, $name);

    echo "Script uploaded successfully..";
    
    //echo $filepath;
}


// if(isset($_FILES['file']['name'])) {
//     $uploaddir = "audio/";
//     $file = basename($_FILES['file']['name']);
//     $uploadfile = $uploaddir . $file;
//     if(file_exists($uploadfile)) {
//         unlink($uploadfile);
//     }
//     if (move_uploaded_file($_FILES['file']['name'], $uploadfile)) {
//       //  $saveData['IMAGE_URL'] = $uploadfile;
//     }
// }

?>