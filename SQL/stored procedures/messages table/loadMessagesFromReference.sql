SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		James
-- Create date: 24/02/2026
-- Description:	load messages with id less or equal than a reference
-- =============================================
CREATE PROCEDURE dbo.loadMessagesFromReference
	-- Add the parameters for the stored procedure here
	@roomid INT = NULL,
	@messageidreference INT = NULL,
	@quantity INT = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT TOP(@quantity) id, senderid, content, time
	FROM dbo.messages
	WHERE roomid = @roomid AND id <= @messageidreference
	ORDER BY id DESC;
END
GO
