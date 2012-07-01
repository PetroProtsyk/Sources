<?php
    $duration      = 10000; // duration in ms
    $fade_duration = 500;   // image change time in ms
    $path = "images\\";

    $exclude_list = array(".", "..");
    $images = array_diff(scandir($path), $exclude_list);

    $images_size = sizeof($images);  

    $single_image_duration = ($duration/$images_size-$fade_duration)/$duration*100;
    $fade_percent = $fade_duration/$duration*100;

    $index = 0;
    $first_image = "";
    $current_precent = $single_image_duration;

    $key_frames = "";

    foreach ($images as $k => $v) {
      $v = $path.$v;
      if ($index == 0)
      {
        $first_image = $v;
        $key_frames.= "0%"."{background:url(".$v.");}\r\n";
        $key_frames.= round($current_precent)."%"."{background:url(".$v.");}\r\n";
      }     
      else
      {
        $current_precent += $fade_percent; 
        $key_frames.= round($current_precent)."%"."{background:url(".$v.");}\r\n";

        $current_precent += $single_image_duration; 
        $key_frames.= round($current_precent)."%"."{background:url(".$v.");}\r\n";
      }
      $index++;
    }
    $key_frames.= "100%"."{background:url(".$first_image.");}\r\n";
?>

<html>
<head>
<style type="text/css"> 
p
{
width:320px;
height:320px;

border-style:solid;
border-width:1px;

background:url(<?php echo $first_image; ?>);
background-repeat:no-repeat;
background-size: 180px 220px;

/*background-attachment:fixed;
background-position:center;*/

animation: image_frames <?php echo $duration; ?>ms linear 1s infinite normal;
-moz-animation: image_frames <?php echo $duration; ?>ms linear 1s infinite normal;
-webkit-animation: image_frames <?php echo $duration; ?>ms linear 1s infinite normal;
}

@keyframes image_frames
{
<?php echo $key_frames; ?>
}

@-moz-keyframes image_frames /* Firefox */
{
<?php echo $key_frames; ?>
}

@-webkit-keyframes image_frames /* Safari and Chrome */
{
<?php echo $key_frames; ?>
}
</style>
</head>

<body>
<p></p>
</body>
</html>