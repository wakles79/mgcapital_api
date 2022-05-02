export class RevenueCSV {
    public contractNumber: number;
    public date: Date;
    public subTotal: number;
    public tax: number;
    public total: number;
    public description: string;
    public reference: string;

    public transactionNumber: string;
    public isRepeated: boolean; 
    public row: number;
}
