   M S B A M L   `   `   `  ���� N  JTHBWiki轮播工具, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:THBWiki轮播工具.libraryTHBWiki轮播工具.library  ,    &THBWiki轮播工具.library.InfoViewer   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     ;local)clr-namespace:THBWiki轮播工具.library     -     � Title$  
InfoViewer��$	��190��$	��750��.��� .+�� $
��Left=�$	��200��.+��� $��这里是封面��$��Center=�$��0q�$��Center=�$��20(��� -    ��Name$ 
VideoImage��� $��	200,0,0,0q�.+�� $��0q�$��70��$	��Top=�.+��� -    ��Name$ 	VideoName��$B��<这里是视频名称，这么长怎么说应该够摆了吧��$��Center=�$��0,5,0,0q�$	��Top=�$��20(��� -   $ VideoAV��$��这里是视频AV号��$��-50,0,120,0q�$��Bottom=�$��Center=�$��14(��� -   $ VideoBV��$��这里是视频BV号��$��250,0,120,0q�$��Bottom=�$��Center=�$��14(��� -   $ 
VideoTname��$��这里是分区��$��Right=�$��0,0,5,0q�$��Bottom=�� $��0,70,0,0q�.+��� $��播放 :��$
��Left=�$��	10,10,0,0q�$	��Top=�$��14(��� -   $ 	VideoView��$��0��$
��Left=�$��	55,10,0,0q�$	��Top=�$��14(��� $��点赞 :��$
��Left=�$��
125,10,0,0q�$	��Top=�$��14(��� -   $ 	VideoLike��$��0��$
��Left=�$��
170,10,0,0q�$	��Top=�$��14(��� $��弹幕 :��$
��Left=�$��	10,30,0,0q�$	��Top=�$��14(��� -	   $ VideoDanmaku��$��0��$
��Left=�$��	55,30,0,0q�$	��Top=�$��14(��� $��投币 :��$
��Left=�$��
125,30,0,0q�$	��Top=�$��14(��� -
   $ 	VideoCoin��$��0��$
��Left=�$��
170,30,0,0q�$	��Top=�$��14(��� $��评论 :��$
��Left=�$��	10,50,0,0q�$	��Top=�$��14(��� -   $ 
VideoReply��$��0��$
��Left=�$��	55,50,0,0q�$	��Top=�$��14(��� $��收藏 :��$
��Left=�$��
125,50,0,0q�$	��Top=�$��14(��� -   $ VideoFavorite��$��0��$
��Left=�$��
170,50,0,0q�$	��Top=�$��14(��� $��分享 :��$
��Left=�$��
240,50,0,0q�$	��Top=�$��14(��� -   $ 
VideoShare��$��0��$
��Left=�$��
285,50,0,0q�$	��Top=�$��14(��� $
��UP :��$
��Left=�$��
240,10,0,0q�$	��Top=�$��14(��� -   $ VideoUploader��$��这是上传者��$��
275,10,0,0q�$	��Top=�$
��Left=�$��14(��� -   $ VideoUploaderUID��$��这是上传者UID��$��
275,30,0,0q�$	��Top=�$
��Left=�$��14(�