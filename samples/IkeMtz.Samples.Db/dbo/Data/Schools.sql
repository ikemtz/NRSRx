IF NOT EXISTS (SELECT TOP(1) 1 FROM [Schools])
BEGIN
  INSERT [Schools] ([Id], [Name], [FullName], [TenantId], [CreatedBy])
  VALUES
  (N'3354c4af-2277-4480-9058-0378b1718913', N'Stanford', N'Stanford University', N'STFRD', N'Data Dump'),
  (N'fe3a0904-f3b8-4906-8532-48a54b329e9a', N'Oxford', N'University of Oxford', N'OXFRD', N'Data Dump'),
  (N'faff8a5d-8de8-4cf2-a671-7bc35d4fcf36', N'Harvard', N'Harvard University', N'HRVRD', N'Data Dump'),
  (N'9df6abcd-95f0-44a5-8df6-aa8a357eced8', N'Yale', N'Yale University', N'YALE', N'Data Dump'),
  (N'2461d779-2e47-46fd-8643-b9c2a9c3be16', N'Columbia', N'Columbia University', N'COLUM', N'Data Dump')
END
