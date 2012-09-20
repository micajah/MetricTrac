update ScoreCardDashboard
set ScoreCardPeriodID=m.ScoreCardPeriodID,
	BaselineValue = m.BaselineValue,
	Breakpoint1Value = m.Breakpoint1Value,
	Breakpoint2Value = m.Breakpoint2Value,
	MaxValue = m.MaxValue,
	MinValue = m.MinValue,
	BaselineValueLabel = m.BaselineValueLabel,
	Breakpoint1ValueLabel = m.Breakpoint1ValueLabel,
	Breakpoint2ValueLabel = m.Breakpoint2ValueLabel
from ScoreCardDashboard d
join ScoreCardMetric m on d.InstanceId=m.InstanceId and d.ScoreCardMetricID=m.ScoreCardMetricID


update MetricValue set FrequencyID = v.FrequencyID from MetricValue v