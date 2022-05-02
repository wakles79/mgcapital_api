INSERT INTO WorkOrderNotificationTemplates 

(
PlainTextTemplate, 
RichTextBodyTemplate, 
SubjectTemplate, 
WorkOrderContactTypeId, 
WorkOrderStatusId, 
[Type]
) 

VALUES

-- Email notifications

(
'<p>Attention, Work Order #[WONumber] for [BuildingName] has been cancelled.<br /><br />If at any point you need further information on this matter, you can reference  <a href="[WOFullUrl]"> Work Order #[WONumber].</a><br /><br />Thank you,<br />[Sig]</p>',
'<p>Attention, Work Order #[WONumber] for [BuildingName] has been cancelled.<br /><br />If at any point you need further information on this matter, you can reference  <a href="[WOFullUrl]"> Work Order #[WONumber].</a><br /><br />Thank you,<br />[Sig]</p>',
NULL,
2, -- Operations Manager
4,
1
),

(
'<p>Attention, Work Order #[WONumber] for [BuildingName] has been cancelled.<br /><br />If at any point you need further information on this matter, you can reference  <a href="[WOFullUrl]"> Work Order #[WONumber].</a><br /><br />Thank you,<br />[Sig]</p>',
'<p>Attention, Work Order #[WONumber] for [BuildingName] has been cancelled.<br /><br />If at any point you need further information on this matter, you can reference  <a href="[WOFullUrl]"> Work Order #[WONumber].</a><br /><br />Thank you,<br />[Sig]</p>',
NULL,
3, -- Supervisor
4,
1
),

(
'<p>Attention, Work Order #[WONumber] for [BuildingName] has been cancelled.<br /><br />If at any point you need further information on this matter, you can reference  <a href="[WOFullUrl]"> Work Order #[WONumber].</a><br /><br />Thank you,<br />[Sig]</p>',
'<p>Attention, Work Order #[WONumber] for [BuildingName] has been cancelled.<br /><br />If at any point you need further information on this matter, you can reference  <a href="[WOFullUrl]"> Work Order #[WONumber].</a><br /><br />Thank you,<br />[Sig]</p>',
NULL,
4, -- Office Staff
4,
1
),

-- Push Notifications

(
'[employeeWhoClosed] has cancelled WO-[woNumber] for [buildingName]',
NULL,
'Work Order [woNumber] has been Cancelled!',
2, -- Operations Manager
4,
4
),

(
'[employeeWhoClosed] has cancelled WO-[woNumber] for [buildingName]',
NULL,
'Work Order [woNumber] has been Cancelled!',
3, -- Supervisor
4,
4
),

(
'[employeeWhoClosed] has cancelled WO-[woNumber] for [buildingName]',
NULL,
'Work Order [woNumber] has been Cancelled!',
4, -- Office Staff
4,
4
)
