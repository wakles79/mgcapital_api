/** Represents a company */
export class CompanyModel {
  constructor(
    /** Gets or sets the Name for this company */
    public name: string
  ) { }

  /** Gets or sets the Id of the company */
  private id: number;

  public get Id(): number {
    return this.id;
  }

  public set Id(value: number) {
    this.id = value;
  }

}
