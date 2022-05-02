export class ExpenseCSVModel {

  public type: string;
  public date: Date;
  public amount: number;
  public vendor: string;
  public description: string;
  public reference: string;
  public contractNumber: number;

  public transactionNumber: string;
  public isRepeated: boolean;
  public row: number;
}
