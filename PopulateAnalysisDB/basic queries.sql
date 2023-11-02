select * from GameFiles
select * from SevenTagRoster order by date, round
select * from Games
select * from Variations
select * from Positions order by variationid, ply

-- game roster highlights
select White, Black, str.date, round, Terminator, count(p.ResultingFEN) from Games g, variations v, positions p, SevenTagRoster str 
where p.variationID = v.VariationID and g.MainLine = v.VariationID and str.GameID = g.GameID
group by white, black, str.date, str.round, Terminator
order by str.date, str.Round

-- last position
insert into EndPositionAnalysis (gameID, finalFEN, Terminator) 
(select gameID, resultingFEN, terminator from (
select g.gameID, White, Black, str.date, round, g.Terminator, p.MoveText, p.ResultingFEN from Games g, variations v, positions p, SevenTagRoster str 
where p.variationID = v.VariationID and g.MainLine = v.VariationID and str.GameID = g.GameID and p.Ply = (select count(p2.ResultingFEN) from positions p2
where p2.variationID = v.VariationID and g.MainLine = v.VariationID and str.GameID = g.GameID)
group by g.gameID, white, black, str.date, str.round, p.ResultingFEN, p.MoveText, Terminator
) as t)

select * from endpositionAnalysis

delete from Positions
update games set MainLine = NULL
delete from Variations
delete from SevenTagRoster
delete from Games
delete from GameFiles
