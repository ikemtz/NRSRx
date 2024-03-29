IF NOT EXISTS (SELECT TOP(1) 1 FROM [Courses])
BEGIN
  INSERT [Courses] ([Id], [Department], [Num], [Title], [Description], [PassRate], [AvgScore], [CreatedBy])
  VALUES
  (N'cb283c3a-b633-4e0c-91d8-011862d843f7', N'Italian Language and Literature', N'ITAL 310', N'Dante in Translation', NULL, 0.325, 2.106, N'Data Dump'),
  (N'c11bd1be-a975-4bd6-bee7-01c0d9b56e7d', N'Chemistry', N'CHEM 125a', N'Freshman Organic Chemistry I', NULL, 0.784, 2.614, N'Data Dump'),
  (N'ae8b485f-4d48-4fd0-893e-034a7a5df19e', N'Sociology', N'SOCY 151', N'Foundations of Modern Social Theory', NULL, 0.793, 2.473, N'Data Dump'),
  (N'cdd24291-2d9d-4ead-91e5-078540dc9c5c', N'Physics', N'PHYS 200', N'Fundamentals of Physics I', NULL, 0.59, 2.277, N'Data Dump'),
  (N'e7fab480-872f-4bfd-b4fc-08a0b4ce5a7c', N'Religious Studies', N'RLST 152', N'Introduction to the New Testament History and Literature', NULL, 0.929, 3.172, N'Data Dump'),
  (N'dd9b69f6-86f1-4951-9c0c-0fad283e9c62', N'Geology and Geophysics', N'GG 140', N'The Atmosphere, the Ocean, and Environmental Change', NULL, 0.433, 3.14, N'Data Dump'),
  (N'b7fcd739-7965-4e91-9120-15c703f38fb9', N'History', N'HIST 202', N'European Civilization, 1648-1945', NULL, 0.53, 3.721, N'Data Dump'),
  (N'586bc7a5-43df-432a-85b2-172b8a218a4c', N'Ecology and Evolutionary Biology', N'EEB 122', N'Principles of Evolution, Ecology and Behavior', NULL, 0.808, 3.699, N'Data Dump'),
  (N'b7b3177b-1724-417a-83c9-185fc4db2969', N'Astronomy', N'ASTR 160', N'Frontiers and Controversies in Astrophysics', NULL, 0.307, 2.101, N'Data Dump'),
  (N'fde6192a-a262-41ff-ac37-23174fa980d1', N'English', N'ENGL 291', N'The American Novel Since 1945', NULL, 0.232, 2.104, N'Data Dump'),
  (N'986ee5a5-1395-4d0c-a58d-2751b39cf5ee', N'Psychology', N'PSYC 123', N'The Psychology, Biology and Politics of Food', NULL, 0.282, 2.161, N'Data Dump'),
  (N'6142abc2-d17c-4d02-9ea2-31e1c94b6ac3', N'Classics', N'CLCV 205', N'Introduction to Ancient Greek History', NULL, 0.837, 2.39, N'Data Dump'),
  (N'b08a92c5-eb21-4798-a154-375b65d9311b', N'English', N'ENGL 220', N'Milton', NULL, 0.604, 2.197, N'Data Dump'),
  (N'5c9d22d4-8b40-4726-b169-3a18075cdeea', N'Economics', N'ECON 251', N'Financial Theory', NULL, 0.863, 2.73, N'Data Dump'),
  (N'8ab53427-6521-4d29-a56d-3f78a913f205', N'African American Studies', N'AFAM 162', N'African American History: From Emancipation to the Present', NULL, 0.229, 2.727, N'Data Dump'),
  (N'a092ae61-05bb-49dc-9db0-4e35bee010b2', N'History', N'HIST 116', N'The American Revolution', NULL, 0.957, 2.102, N'Data Dump'),
  (N'71d1f73e-56d7-491b-9664-4f8250dddfa1', N'Physics', N'PHYS 201', N'Fundamentals of Physics II', NULL, 0.345, 2.777, N'Data Dump'),
  (N'b9144a15-652d-471c-b6ca-58610aae1871', N'History', N'HIST 234', N'Epidemics in Western Society Since 1600', NULL, 0.669, 3.532, N'Data Dump'),
  (N'90f2863c-b0ab-4dab-be14-621daeb4d34c', N'Biomedical Engineering', N'BENG 100', N'Frontiers of Biomedical Engineering', NULL, 0.969, 2.417, N'Data Dump'),
  (N'76c2acf0-acab-43a7-8abb-6cdc206ad81f', N'Spanish and Portuguese', N'SPAN 300', N'Cervantes'' Don Quixote', NULL, 0.385, 2.125, N'Data Dump'),
  (N'0bbda808-41df-47be-be4b-7a5d72d655be', N'Environmental Studies', N'EVST 255', N'Environmental Politics and Law', NULL, 0.849, 2.671, N'Data Dump'),
  (N'8ede0f32-7940-4664-960b-7e74a25baf0f', N'Religious Studies', N'RLST 145', N'Introduction to the Old Testament (Hebrew Bible)', NULL, 0.886, 3.546, N'Data Dump'),
  (N'b08af3c5-3868-4abe-b0c2-849003b57467', N'English', N'ENGL 310', N'Modern Poetry', NULL, 0.606, 2.13, N'Data Dump'),
  (N'bb845839-ca00-4fd2-9b0a-9002f63c9bdf', N'History', N'HIST 276', N'France Since 1871', NULL, 0.905, 3.732, N'Data Dump'),
  (N'c814357e-5777-43c4-ac62-a6a86a6a2428', N'Political Science', N'PLSC 270', N'Capitalism: Success, Crisis, and Reform', NULL, 0.593, 2.165, N'Data Dump'),
  (N'80416bdf-ec8b-4f87-9a9b-b72edf34b657', N'Philosophy', N'PHIL 176', N'Death', NULL, 0.419, 2.12, N'Data Dump'),
  (N'c49d56aa-a79b-49c8-81a2-bfc3e07c1445', N'History', N'HIST 251', N'Early Modern England: Politics, Religion, and Society under the Tudors and Stuarts', NULL, 0.569, 3.134, N'Data Dump'),
  (N'0de44c2d-96a9-4b48-843c-c0973822236c', N'Molecular, Cellular and Developmental Biology', N'MCDB 150', N'Global Problems of Population Growth', NULL, 0.848, 2.15, N'Data Dump'),
  (N'4b3a64d1-3a53-42c0-9e5e-c52ac40a8251', N'History', N'HIST 210', N'The Early Middle Ages, 284–1000', NULL, 0.619, 2.192, N'Data Dump'),
  (N'654140d4-700b-49cf-a276-c9c36ffb1374', N'Economics', N'ECON 252', N'Financial Markets (2011)', NULL, 0.236, 3.149, N'Data Dump'),
  (N'c3634746-0410-43e0-acf2-ca5b8dfa3b52', N'Chemistry', N'CHEM 125b', N'Freshman Organic Chemistry II', NULL, 0.608, 2.236, N'Data Dump'),
  (N'68477b23-4ea6-49b2-b94f-cedf5582fac5', N'Psychology', N'PSYC 110', N'Introduction to Psychology', NULL, 0.917, 3.522, N'Data Dump'),
  (N'5bfdd5cc-b09f-4ee4-8d4b-d10520a8ccad', N'History of Art', N'HSAR 252', N'Roman Architecture', NULL, 0.549, 2.168, N'Data Dump'),
  (N'0ded87ea-ace6-43ad-a8f8-d112e60fff61', N'English', N'ENGL 300', N'Introduction to Theory of Literature', NULL, 0.809, 2.142, N'Data Dump'),
  (N'103d3899-f4ac-4f14-98cc-d9d2365f441c', N'American Studies', N'AMST 246', N'Hemingway, Fitzgerald, Faulkner', NULL, 0.417, 3.702, N'Data Dump'),
  (N'b070def6-5a36-4f00-8341-e9158f495acf', N'History', N'HIST 119', N'The Civil War and Reconstruction Era, 1845-1877', NULL, 0.484, 3.101, N'Data Dump'),
  (N'4312b7b3-6103-465f-86f6-eeeaf3e69768', N'Political Science', N'PLSC 114', N'Introduction to Political Philosophy', NULL, 0.381, 3.163, N'Data Dump'),
  (N'e9452025-0775-43da-bd65-f04957ac16b2', N'Economics', N'ECON 159', N'Game Theory', NULL, 0.629, 2.149, N'Data Dump'),
  (N'cdde9500-c129-498e-ad3f-f244ef2f40c4', N'Philosophy', N'PHIL 181', N'Philosophy and the Science of Human Nature', NULL, 0.894, 2.175, N'Data Dump'),
  (N'b715b198-6c4d-4134-aef0-faf0bf859854', N'Economics', N'ECON 252', N'Financial Markets (2008)', NULL, 0.781, 3.519, N'Data Dump')
END
