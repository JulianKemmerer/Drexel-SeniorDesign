   M S B A M L   `   `   `  ����  kinect_tracking     kinect_tracking.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  
MainWindow��5   	   $
��768 ��6   $
��1024��6*    � 
ResizeMode$ NoResize=�67   .��5      � .+�5   
   �� -   5         ��Name$ skeletonCanvas��5      }�. 
���   �$��249,0,0,141q�6�   ��6)   �� *ActualHeight+ �� ElementName$ PartialWindowHorizontal0�� �� Mode$ OneWay=�6�   �� 5	   
   -   5         ��Name$ largeStatusText��5	       �� 
IsReadOnly . $��30(�6=   $��Center=�6K   $��47��6h   $��249,615,0,79q�6t    �� TextAlignment$ Center=�6�    �� TextWrapping$
 Wrap=�6�   $�� ��6�   $��Center=�6�   $	��767��6�   �� 5
   
   $��
Audio Demo��6   $
��Left=�6'   $��
10,637,0,0q�6B   $	��Top=�6V   $	��152��6n   $��94��6z   .��5      �� $
��Left=�6   $��99��64   $	��Top=�6@   $	��140��6X   $��0,10,0,0q�6d   .+�5      �� -   5        	 ��Name$	 selectAudioButton��5      $
��Left=�64   $	��Top=�6O   
 J� RenderTransformOrigin$
 	-0.16,4.3&�6h   $��20��6�   $��Select Audio��6�   $	��140��6�   �� 5      -   5        $	 
playButton��5      $
��Play��6-   $
��Left=�6<   $	��Top=�6W   $	��140��6o   6{   �� 5      -   5        $	 pauseButton��5      $��Pause��6.   $
��Left=�6>   $	��Top=�6Y   $	��140��6q   6}   5      5      �� 5   
   $��Gesture Information��6   $
��Left=�60   $��249,667,0,0q�6K   $	��Top=�6`   $��74��6x   $	��767��6�   .��5      &   System.Windows.Controls.DataGrid  -   5          Name$ gestureDataGrid��5         
IsReadOnly . $��0,10,0,0q�6B   5      �� 5   
   -   5         ��Name$ 
grpGesture��5      $��Gesture Capture��6'   $
��Left=�6@   $��
10,118,0,0q�6[   $	��Top=�6o   $	��206��6�   $	��213��6�   .��5      �� $
��Left=�6   $	��176��64   $	��Top=�6A   $	��201��6Y   $��0,10,0,0q�6e   .+�5      �� -	   5        $ txtGestureName��5      $
��Left=�62   $��23��6M   $
 Wrap=�6Y   $��Enter gesture name ...��6m   $	��Top=�6�   $	��201��6�   �� 5      -
   5         ��Name$ cmbbSkeletonCaptureSelect��5       �� 
IsReadOnly . {�.  6P   �� 5      -   5        $	 btnCaptureGesture��5      $��Begin Capture��64   $
��Left=�6L   $	��Top=�6g   $	��201��6   {�.  6�   �� 5      -   5        $	 btnSaveGestureToFile��5      $��Save to File��67   {�.  6N   �� 5      -   5        $	 btnLoadGestureFromFile��5      $��Load from File��69   $
��Left=�6R   $	��Top=�6m   $	��201��6�   {�.  6�   5      5      �� 5   
   $��Kinect Information��6   $
��Left=�6/   $��	10,10,0,0q�6J   $	��Top=�6]   $	��103��6u   $	��213��6�   .��5      �� $
��Left=�6   $	��Top=�65   $��75��6M   $	��201��6Y   $��0,10,0,0q�6e   .+�5      �� -   5        $	 btnConnectFirstKinect��5      $��Connect First Kinect��68   $
��Left=�6W   $	��Top=�6r   $	��201��6�   $��20��6�   $
 -6.282,-1.42&�6�   5       5!      5$      5%      