alter table t_question
    add binary_split binary default 0
go

alter table t_answer
    add sub_question_id int default 0
go

exec sp_rename 't_answer.sub_question_id', journey, 'COLUMN'
go

-- auto-generated definition
create type type_answer_v4 as table
(
    question_id      int,
    answer_text      varchar(max),
    option_id        int,
    next_question_id int,
    display_index    int,
    options_order    varchar(64),
    display_time     datetime,
    selection_time   datetime,
    journey          int
)
go

CREATE PROCEDURE [dbo].[submit_answers_v4]
    @mobile varchar(64),
	@nasid int,
	@template_id int,
	@answers type_answer_v4 READONLY
AS
    SET NOCOUNT ON;

	INSERT INTO t_answer(question_id, template_id, mobile, answer_text, option_id, next_question_id, router_nas_id, display_index, options_order, display_time, selection_time, journey)
	SELECT question_id, @template_id, @mobile, answer_text, option_id, next_question_id, @nasid, display_index, options_order, display_time, selection_time, journey FROM @answers

	SELECT 1 AS status;
go