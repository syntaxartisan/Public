UPDATE ListReportDetails SET HiddenValues=
CONCAT('grade_from=',
CASE SUBSTR(SelectionCriteriaFilterText,LOCATE('from',SelectionCriteriaFilterText)+5,LOCATE('to',SelectionCriteriaFilterText)-(LOCATE('from',SelectionCriteriaFilterText)+6))
WHEN 'KG' THEN 'K0'
WHEN 'KA' THEN 'K1'
WHEN 'KB' THEN 'K2'
WHEN 'KC' THEN 'K3'
WHEN 'KD' THEN 'K4'
WHEN 'EC' THEN 'K5'
WHEN 'HK' THEN 'K6'
ELSE
SUBSTR(SelectionCriteriaFilterText,LOCATE('from',SelectionCriteriaFilterText)+5,LOCATE('to',SelectionCriteriaFilterText)-(LOCATE('from',SelectionCriteriaFilterText)+6))
END,
';grade_to=',
CASE SUBSTR(SelectionCriteriaFilterText,LOCATE('to',SelectionCriteriaFilterText)+3,LENGTH(SelectionCriteriaFilterText)-(LOCATE('to',SelectionCriteriaFilterText)+2))
WHEN 'KG' THEN 'K0'
WHEN 'KA' THEN 'K1'
WHEN 'KB' THEN 'K2'
WHEN 'KC' THEN 'K3'
WHEN 'KD' THEN 'K4'
WHEN 'EC' THEN 'K5'
WHEN 'HK' THEN 'K6'
ELSE
SUBSTR(SelectionCriteriaFilterText,LOCATE('to',SelectionCriteriaFilterText)+3,LENGTH(SelectionCriteriaFilterText)-(LOCATE('to',SelectionCriteriaFilterText)+2))
END,
';')
WHERE CategoryItemId=9 AND CategoryCode='F'
AND LOCATE('from',SelectionCriteriaFilterText)>0
AND LOCATE('to',SelectionCriteriaFilterText)>0;