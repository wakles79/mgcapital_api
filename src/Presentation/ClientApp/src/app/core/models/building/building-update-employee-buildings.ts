export class BuildingUpdateEmployeeBuildingsModel {
  employeeId: number;
  type: number;
  buildingsId: number[];

  constructor(buildingUpdate: BuildingUpdateEmployeeBuildingsModel) {
    this.employeeId = buildingUpdate.employeeId || 0;
    this.type = buildingUpdate.type || 0;
    this.buildingsId = buildingUpdate.buildingsId || [];
  }
}
