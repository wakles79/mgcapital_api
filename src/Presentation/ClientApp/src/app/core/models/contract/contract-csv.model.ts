import { ContractOfficeSpaceModel } from './contract-office-space.model';

export class ContractCSVModel {
    ContractNumber: string;
    Area: number;
    NumberOfPeople: number;
    BuildingId: number;
    CustomerId: number;
    ContactSignerId: number;
    Description: string;
    DaysPerMonth: number;
    NumberOfRestrooms: number;
    FrequencyPerYear: number;
    ExpirationDate: Date;
    Status: number;

    ProductionRate: number;
    UnoccupiedSquareFeets: number;
    OfficeSpaces: ContractOfficeSpaceModel[];
    EditionCompleted: boolean;

    BuildingCode: string;
    CustomerCode: string;
  }
