-- =============================================
-- Script to drop and reload all enumeration lookup table data
-- =============================================

/* PROTOCOL */
EXEC pr_Disable_Foreign_Keys_Triggers 1, 'Protocol'
DELETE FROM Protocol
INSERT INTO Protocol ([Name],Id) VALUES ('Mining', '32f8b380-8436-461b-b1fb-837baea88830')
INSERT INTO Protocol ([Name],Id) VALUES ('Airports', '6eb1f8c2-d3d9-4b74-98cf-6c33737a2dc0')
EXEC pr_Disable_Foreign_Keys_Triggers 0, 'Protocol'

/* GROUP CATEGORY ASPECT */
EXEC pr_Disable_Foreign_Keys_Triggers 1, 'GroupCategoryAspect'
DELETE FROM GroupCategoryAspect
INSERT INTO GroupCategoryAspect ([Name],Id,InstanceId,ParentId) VALUES ('GRI','45befb09-bcb5-46c7-9d61-d37f554b4967','00000000-0000-0000-0000-000000000000',null)
	INSERT INTO GroupCategoryAspect ([Name],Id,InstanceId,ParentId) VALUES ('Economic','b2cb1c87-4bc8-4a91-bd54-4a765f5d417d','00000000-0000-0000-0000-000000000000','45befb09-bcb5-46c7-9d61-d37f554b4967')
	INSERT INTO GroupCategoryAspect ([Name],Id,InstanceId,ParentId) VALUES ('Environmental','8c1dd8eb-2d02-4ff8-ad09-11045ddeef68','00000000-0000-0000-0000-000000000000','45befb09-bcb5-46c7-9d61-d37f554b4967')
		INSERT INTO GroupCategoryAspect ([Name],Id,InstanceId,ParentId) VALUES ('Biodiversity','15b8addf-6fe9-483a-9755-46550731f152','00000000-0000-0000-0000-000000000000','8c1dd8eb-2d02-4ff8-ad09-11045ddeef68')
		INSERT INTO GroupCategoryAspect ([Name],Id,InstanceId,ParentId) VALUES ('Compliance','4c626242-f0ca-4e1c-b0df-6bd650a16691','00000000-0000-0000-0000-000000000000','8c1dd8eb-2d02-4ff8-ad09-11045ddeef68')
		INSERT INTO GroupCategoryAspect ([Name],Id,InstanceId,ParentId) VALUES ('Emissions','5110dd10-65b9-44e2-9af5-412f8465010d','00000000-0000-0000-0000-000000000000','8c1dd8eb-2d02-4ff8-ad09-11045ddeef68')
		INSERT INTO GroupCategoryAspect ([Name],Id,InstanceId,ParentId) VALUES ('Effluents and Waste','a749890d-87c3-47bc-a4d0-f4fb8dd42233','00000000-0000-0000-0000-000000000000','8c1dd8eb-2d02-4ff8-ad09-11045ddeef68')
		INSERT INTO GroupCategoryAspect ([Name],Id,InstanceId,ParentId) VALUES ('Energy','0e38de90-ca2e-4ea4-bf2d-223bf91ec928','00000000-0000-0000-0000-000000000000','8c1dd8eb-2d02-4ff8-ad09-11045ddeef68')
		INSERT INTO GroupCategoryAspect ([Name],Id,InstanceId,ParentId) VALUES ('Materials','19841059-45cf-49c6-82de-dda5ab49d840','00000000-0000-0000-0000-000000000000','8c1dd8eb-2d02-4ff8-ad09-11045ddeef68')
		INSERT INTO GroupCategoryAspect ([Name],Id,InstanceId,ParentId) VALUES ('Overall (Environmental)','881b3002-d37a-4a42-b008-251eef1ba863','00000000-0000-0000-0000-000000000000','8c1dd8eb-2d02-4ff8-ad09-11045ddeef68')
		INSERT INTO GroupCategoryAspect ([Name],Id,InstanceId,ParentId) VALUES ('Products and Services','b3d7d81a-6d09-4b8b-b505-096fc5abebe1 ','00000000-0000-0000-0000-000000000000','8c1dd8eb-2d02-4ff8-ad09-11045ddeef68')
		INSERT INTO GroupCategoryAspect ([Name],Id,InstanceId,ParentId) VALUES ('Transport','87ec99cb-d30a-45f7-a0d0-4e99880eb7d5','00000000-0000-0000-0000-000000000000','8c1dd8eb-2d02-4ff8-ad09-11045ddeef68')
		INSERT INTO GroupCategoryAspect ([Name],Id,InstanceId,ParentId) VALUES ('Water','a752095b-d9e0-4c7e-9c5d-5debee23158b','00000000-0000-0000-0000-000000000000','8c1dd8eb-2d02-4ff8-ad09-11045ddeef68')
	INSERT INTO GroupCategoryAspect ([Name],Id,InstanceId,ParentId) VALUES ('Social Performance: Labor Practices & Decent Work','c2d444e1-8534-46ed-bfad-5d5b6dfd5199','00000000-0000-0000-0000-000000000000','45befb09-bcb5-46c7-9d61-d37f554b4967')
	INSERT INTO GroupCategoryAspect ([Name],Id,InstanceId,ParentId) VALUES ('Social Performance: Human Rights','93d6b19e-c513-4a46-b75a-adc5fbe4e009','00000000-0000-0000-0000-000000000000','45befb09-bcb5-46c7-9d61-d37f554b4967')
	INSERT INTO GroupCategoryAspect ([Name],Id,InstanceId,ParentId) VALUES ('Social Performance: Society','ea90466f-bd25-4e1a-bb03-d52eb00d6762','00000000-0000-0000-0000-000000000000','45befb09-bcb5-46c7-9d61-d37f554b4967')
	INSERT INTO GroupCategoryAspect ([Name],Id,InstanceId,ParentId) VALUES ('Social Performance: Product Responsibility','738ab4a9-3812-4296-b679-3a5994d2b77d','00000000-0000-0000-0000-000000000000','45befb09-bcb5-46c7-9d61-d37f554b4967')
EXEC pr_Disable_Foreign_Keys_Triggers 0, 'GroupCategoryAspect'





 
 
 
 
 
