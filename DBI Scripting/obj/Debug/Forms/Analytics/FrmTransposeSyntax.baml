   M S B A M L   `   `   `  ����  DBI Scripting6    0DBI_Scripting.Forms.Analytics.FrmTransposeSyntax   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5   	     � Title$  FrmTransposeSyntax��5   	   $��470.237��6$   $��866.136��65    � WindowStartupLocation$ CenterScreen=�6E   .��5      � $��0,1,0.333,-0.667q�6   .+�5   
   �� $&�� Imagery Transpose & Table Syntax��6   $
��Left=�6A   $��	27,10,0,0q�6\   $	��Top=�6o   $	��184��6�   $	��809��6�   .��5      � .+�5	      �� $��Image Var Path :��6   $
��Left=�63   $��	12,11,0,0q�6N   $	��Top=�6a    J� RenderTransformOrigin$ -0.813,-0.45&�6y   $��96��6�   �� 5
      -   5         ��Name$ txtImageVarFile��5
      $
��Left=�63   $��23��6N   $��
114,13,0,0q�6Z    �� TextWrapping$
 Wrap=�6n   $�� ��6�   $	��Top=�6�   $	��575��6�   �� 5      -   5         ��Name$ btnBrowseImage��5      $��Browse��61   $
��Left=�6B   $��
712,13,0,0q�6]   $	��Top=�6q   $��22��6�   $��75��6�   6�   �� 5      -   5        $ btnRunTranspose��5      $	��Run��62   $
��Left=�6@   $��
354,84,0,0q�6[   $	��Top=�6o   $	��111��6�   $��33��6�   6�   �� 5      $	��Top=�6   $ -3.74,-2.112&�60   $��
165,43,0,0q�6U   $
��Left=�6i   $��	WrokSheet��6�   $  Xceed.Wpf.Toolkit.CheckListBox  5      -   5          Name$ chkListBoxWorksheet_Copy��5   $   $
��Left=�6F   $��83��6a   $��115,70,0,-11q�6m   $	��Top=�6�   $	��179��6�   5      5      �� 5   
   $#��Rank Transpose & Table Syntax��6   $
��Left=�6>   $��
27,199,0,0q�6Y   $	��Top=�6m   $	��186��6�   $	��809��6�   .��5      �  � ColumnDefinitions 5      �� 5      $	��35* �6'   �� 5      $
��762* �6'   .+�5      �� $��Rank Var Path :��6   $
��Left=�62   $��	12,11,0,0q�6M   $	��Top=�6`   $ -0.813,-0.45&�6x   $��96��6�   $��2��6�   �� 5      -   5        $ txtRankExcelFile��5      $
��Left=�64   $��23��6O   $��	79,13,0,0q�6[   $
 Wrap=�6n   $�� ��6�   $	��Top=�6�   $	��575��6�   $��1��6�   �� 5      -   5        $ btnBrowseRank��5      $��Browse��60   $
��Left=�6A   $��
677,13,0,0q�6\   $	��Top=�6p   $��22��6�   $��75��6�   $��1��6�   �� 5      -   5        $ 
btnRunRank��5      $	��Run��6-   $
��Left=�6;   $��318.667,116,0,0q�6V   $	��Top=�6o   $	��111��6�   $��33��6�   $��1��6�   �� 5      $	��Top=�6   $ -3.74,-2.112&�60   $��
130,43,0,0q�6U   $
��Left=�6i   $��	WrokSheet��6�   $��1��6�     5      -   5        $ chkListBoxWorksheetRank��5   $   $
��Left=�6E   $��83��6`   $��	80,70,0,0q�6l   $	��Top=�6   $	��179��6�   $��1��6�   �� 5      -	   5        $ btnRunSetStructure��5      $
��Set ��65   $
��Left=�6D   $��318.667,70,0,0q�6_   $	��Top=�6w   $	��111��6�   $��33��6�   $��1��6�   5      5      �� 5    
   -
   5        $ btnExit��5       $
��Exit��6"   $
��Left=�61   $��406,396,0,0q�6L   $	��Top=�6a   $��22��6y   $��75��6�   $ 0.285,-1.116&�6�   6�   5"      5#      