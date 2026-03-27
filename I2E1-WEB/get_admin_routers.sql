USE [i2e1_test1]
GO

/****** Object:  StoredProcedure [dbo].[get_admin_routers]    Script Date: 10/6/2016 5:11:58 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




CREATE PROCEDURE [dbo].[update_t_store]
	@name VARCHAR(MAX),
	@address VARCHAR(MAX),
	@city VARCHAR(MAX),
	@state VARCHAR(MAX),
	@latitude VARCHAR(MAX),
	@longitude VARCHAR(MAX),
	@contact VARCHAR(MAX),
	@email VARCHAR(MAX),
	@install_state int,
	@category VARCHAR(MAX),
	@nasid int,
	@location VARCHAR(MAX),
	@controllerid int,
	@partner VARCHAR(MAX)

AS 
    SET NOCOUNT ON;

	update t_store set 
	shop_name=@name, 
	shop_address=@address,
	shop_city=@city, 
	shop_state=@state, 
	contact_number=@contact,
	email=@email, 
	install_state=@install_state, 
	latitude=@latitude, 
	longitude=@longitude, 
	category=@category,
	partner_id=@partner 
	where router_nas_id=@nasid

	update t_controller set location=@location where router_nas_id=@nasid and controller_id=@controllerid





GO


