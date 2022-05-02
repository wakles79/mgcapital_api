export interface GmailTicket {
  attachments: any[];
  body: string;
  bodyText: string;
  createdDate: Date;
  date: string;
  deliveredTo: string;
  from: string;
  fromName: string;
  headerMessageID: string;
  inReplyTo: string;
  messageId: string;
  references: string;
  replyTo: string;
  subject: string;
  threadTopic: string;
  to: string;
}
