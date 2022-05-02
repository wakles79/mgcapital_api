import { EntityModel } from '@core/models/common/entity.model';

export class RevenueExpenseCSVModel {
  public description: string;
  public date: Date;
  public reference: string;
  public type: number;
  public typeName: string;
  public creditAmount: number;
  public debitAmount: number;

  public isRepeated: boolean;
  public transactionNumber: string;
  public row: number;
}
