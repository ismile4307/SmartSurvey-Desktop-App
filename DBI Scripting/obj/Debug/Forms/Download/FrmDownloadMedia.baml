   M S B A M L   `   `   `  ����  DBI Scripting3    -DBI_Scripting.Forms.Download.FrmDownloadMedia   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          �Name$  frmDownloadMedia��5   	    � Title$ Downlaod Media��5   	   $��278.293��6    $��827.93��61    � WindowStartupLocation$ CenterScreen=�6@    � 
ResizeMode$ NoResize=�6e   .��5      � .+�5   
   � -   5         �Name$ progressbar1��5      $
��Left=�6,   $��25��6G   $��	47,10,0,0q�6S   $	��Top=�6f   $	��723��6~   �� 5   
   $��Data file Path :��6   $
��Left=�6+   $��	47,78,0,0q�6F   $	��Top=�6Y    J� RenderTransformOrigin$ -3.74,-2.112&�6q   �� 5   
   -   5         ��Name$ txtDataFileLocation��5      $
��Left=�6/   $��23��6J   $��
139,81,0,0q�6V    �� TextWrapping$
 Wrap=�6j   $�� ��6~   $	��Top=�6�   $	��551��6�   �� 5	   
   -   5         ��Name$ btnBrowseDataFile��5	      $��Browse��6,   $
��Left=�6=   $��
695,81,0,0q�6X   $	��Top=�6l   $��75��6�   $ 8.079,-5.321&�6�   6�   �� 5
   
   $��Save Location :��6   $
��Left=�6*   $��
42,111,0,0q�6E   $	��Top=�6Y   $ -3.74,-2.112&�6q   �� 5   
   -   5        $ txtSaveLocation��5      $
��Left=�6+   $��23��6F   $��139,114,0,0q�6R   $
 Wrap=�6g   $�� ��6{   $	��Top=�6�   $	��551��6�   �� 5   
   $��Browse��6   $
��Left=�6"   $��695,114,0,0q�6=   $	��Top=�6R   $��75��6j   $ 8.079,-5.321&�6u   �� 5   
   -   5        $ btnDownload��5      $��Download��6&   $
��Left=�69   $��372,153,0,0q�6T   $	��Top=�6i   $	��108��6�   $��31��6�   6�   �� 5   
   -   5        $ btnClose��5      $��Close��6#   $
��Left=�63   $��391,200,0,0q�6N   $	��Top=�6c   $��75��6{   $ 8.079,-5.321&�6�   6�   �� 5   
   $��Media Type :��6   $
��Left=�6'   $��
52,143,0,0q�6B   $	��Top=�6V   $ -3.74,-2.112&�6n   �� 5   
   -   5        	 ��Name$	 comMediaType��5      $
��Left=�6)   $��139,146,0,0q�6D   $	��Top=�6Y   $	��120��6q   6}   �� 5   
   $�� ��6   $
��Left=�6   $��496,142,0,0q�68   $	��Top=�6M   $��83��6e   $	��275��6q   .��5      �� -	   5        
 ��Name$
 lblDownloadStatus��5      $��Downloaded : ��6/   $
��Left=�6G   $��	10,16,0,0q�6b   $	��Top=�6u   5      �� 5   
   $��Project Name :��6   $
��Left=�6)   $��	46,45,0,0q�6D   $	��Top=�6W   $ -3.74,-2.112&�6o   �� 5   
   -
   5        $	 comProjectName��5      $
��Left=�6+   $��
139,48,0,0q�6F   $	��Top=�6Z   $	��166��6r   6~   �� 5   
   -   5        $ btnKillProcess��5      $��Kill Process��6)   $
��Left=�6@   $��
22,210,0,0q�6[   $	��Top=�6o   $��75��6�   $ 0.041,0.259&�6�   6�   5      5      