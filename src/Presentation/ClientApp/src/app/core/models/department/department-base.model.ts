import { CompanyEntity } from '@core/models/common/company-entity.model';

export class DepartmentBaseModel extends CompanyEntity {
  name: string;

  constructor(department: DepartmentBaseModel) {
    super(department);
    if (department == null) {
      this.id = -1;
      this.guid = '';
      this.name = '';
    }
    else {
      this.id = department.id;
      this.guid = department.guid;
      this.name = department.name;
    }
  }

}
