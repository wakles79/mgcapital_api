import { RevenueBaseModel } from '@core/models/revenue/revenue-base.model';

export class RevenueGridModel extends RevenueBaseModel {

  buildingName: string;
  customerName: string;
  epochDate: number;
  constructor(revenueGridModel: RevenueGridModel) {
    super(revenueGridModel);

    this.buildingName = revenueGridModel.buildingName || '';
    this.customerName = revenueGridModel.customerName || '';
    this.epochDate = revenueGridModel.epochDate || 0;
  }
}
