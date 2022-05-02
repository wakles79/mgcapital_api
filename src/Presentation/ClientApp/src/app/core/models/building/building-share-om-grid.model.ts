export class BuildingShareOperationManagerGridModel {

  /** Building Id */
  id: number;
  /** Building Name */
  name: string;
  /** Is Shared with another Operation Manager */
  isShared: boolean;
  /** Type of Employee */
  type: number;
  /** Name of the type */
  currentTypeName: string;
  /** Used only on view */
  isUpdated: boolean;

  constructor() {

  }
}
