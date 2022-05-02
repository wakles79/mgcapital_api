import { BuildingBaseModel } from './building-base.model';

export class BuildingUpdateEmployeesModel extends BuildingBaseModel {

  private operationsManagers: any[];
  private inspectorId: number;
  private supervisors: any[];

  setEmployees(): void {

    this.employees = [];

    // add 'type' = 1 to all supervisors
    this.supervisors.forEach(element => {
      element['type'] = 1;
    });
    this.employees = this.supervisors;

    this.operationsManagers.forEach(element => {
      const op = {
        roleName: element.roleName,
        id: element.id,
        name: element.name,
        type: 2,
      };

      this.employees.push(op);
    });

    // const om = {
    //   roleName: '',
    //   id: this.operationsManagerId,
    //   name: '',
    //   type: 2,
    // };
    // this.employees.push(om);

    if (this.inspectorId) {
      const inspector = {
        roleName: '',
        id: this.inspectorId,
        name: '',
        type: 8,
      };
      this.employees.push(inspector);
    }
  }
  constructor(building: BuildingUpdateEmployeesModel) {
    super(building);
    this.inspectorId = building.inspectorId || null;
    this.supervisors = building.supervisors;
    this.operationsManagers = building.operationsManagers;
    this.setEmployees();
  }
}
