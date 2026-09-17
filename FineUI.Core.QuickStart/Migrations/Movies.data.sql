SET IDENTITY_INSERT [dbo].[Movies] ON
INSERT INTO [dbo].[Movies] ([ID], [Title], [ReleaseDate], [Genre], [Price]) VALUES (1, N'回到未来', N'1985-07-03 00:00:00', N'科幻', CAST(8.99 AS Decimal(18, 2)))
INSERT INTO [dbo].[Movies] ([ID], [Title], [ReleaseDate], [Genre], [Price]) VALUES (11, N'回到未来2', N'1989-11-22 00:00:00', N'科幻', CAST(10.99 AS Decimal(18, 2)))
INSERT INTO [dbo].[Movies] ([ID], [Title], [ReleaseDate], [Genre], [Price]) VALUES (12, N'星球大战', N'1977-05-25 00:00:00', N'科幻', CAST(12.99 AS Decimal(18, 2)))
INSERT INTO [dbo].[Movies] ([ID], [Title], [ReleaseDate], [Genre], [Price]) VALUES (14, N'星球大战2：帝国反击战', N'1980-05-21 00:00:00', N'科幻', CAST(16.99 AS Decimal(18, 2)))
INSERT INTO [dbo].[Movies] ([ID], [Title], [ReleaseDate], [Genre], [Price]) VALUES (15, N'星球大战3：绝地归来', N'1983-05-25 00:00:00', N'科幻', CAST(20.99 AS Decimal(18, 2)))
INSERT INTO [dbo].[Movies] ([ID], [Title], [ReleaseDate], [Genre], [Price]) VALUES (16, N'星球大战8：最后的绝地武士', N'2018-01-05 00:00:00', N'科幻', CAST(50.99 AS Decimal(18, 2)))
INSERT INTO [dbo].[Movies] ([ID], [Title], [ReleaseDate], [Genre], [Price]) VALUES (17, N'星球大战9：天行者崛起', N'2019-12-18 00:00:00', N'科幻', CAST(88.99 AS Decimal(18, 2)))
SET IDENTITY_INSERT [dbo].[Movies] OFF
