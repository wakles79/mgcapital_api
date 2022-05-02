import { EntityModel } from '../common/entity.model';

export class FdAttachmentBaseModel extends EntityModel {
  public content_type: string;
  public size: number;
  public name: string;
  public attachment_url: string;
  public created_at: Date;
  public updated_at: Date;

  constructor(attatchment) {
    super(attatchment);
    this.content_type = attatchment.content_type || null;
    this.size = attatchment.size || null;
    this.name = attatchment.name || null;
    this.attachment_url = attatchment.attachment_url || null;
    this.created_at = attatchment.created_at || null;
    this.updated_at = attatchment.updated_at || null;
  }
}
