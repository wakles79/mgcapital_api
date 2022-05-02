import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { fuseAnimations } from '@fuse/animations';
import * as Papa from 'papaparse';

@Component({
  selector: 'app-result-import-csv',
  templateUrl: './result-import-csv.component.html',
  styleUrls: ['./result-import-csv.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class ResultImportCsvComponent implements OnInit {

  value: any;
  type: any;
  SaveItems = 0;
  RepeatedItems = 0;
  Error = 0;
  ContractCodeNotFound = 0;
  BuildingCodeNotFound = 0;
  CustomerCodeNotFound = 0;

  constructor(
    public dialogRef: MatDialogRef<ResultImportCsvComponent>,
    @Inject(MAT_DIALOG_DATA) private data: any,
    private snackBar: MatSnackBar,
    private formBuilder: FormBuilder
  ) {
    this.value = data.result;
    this.type = data.type;
    console.log(data.itemResult);
    this.SaveItems = this.value[0];
    this.RepeatedItems = this.value[5];
    this.Error = this.value[6];
    this.ContractCodeNotFound = this.value[3];
    this.BuildingCodeNotFound = this.value[2];
    this.CustomerCodeNotFound = this.value[1];

    switch (this.type) {
      //   case 'budget':
      //     this.SaveItems = this.value[0];
      //     this.RepeatedItems = this.value[3];
      //     this.Error = this.value[6];
      //     this.ContractCodeNotFound = 0;
      //     this.BuildingCodeNotFound = this.value[2];
      //     this.CustomerCodeNotFound = this.value[1];
      //     break;
      //   case 'budgetItem':
      //     this.SaveItems = this.value[0];
      //     this.RepeatedItems = this.value[2];
      //     this.Error = this.value[4];
      //     this.ContractCodeNotFound = this.value[1];
      //     this.BuildingCodeNotFound = 0;
      //     this.CustomerCodeNotFound = 0;
      //     break;
      case 'Revenues&Expenses':
        this.SaveItems = this.value[0];
        this.RepeatedItems = this.value[2];
        this.Error = this.value[3];
        this.ContractCodeNotFound = this.value[1];
        this.BuildingCodeNotFound = 0;
        this.CustomerCodeNotFound = 0;
        break;
      //   case 'budgetExpense':
      //     this.SaveItems = this.value[0];
      //     this.RepeatedItems = 0;
      //     this.Error = this.value[3];
      //     this.ContractCodeNotFound = this.value[2];
      //     this.BuildingCodeNotFound = 0;
      //     this.CustomerCodeNotFound = 0;
      //     break;
      default:
        break;
    }
  }

  ngOnInit(): void {
    this.downloadCSV();
  }

  downloadCSV(): void {
    const csv = Papa.unparse(this.data.itemResult);

    const csvData = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
    let csvURL = null;
    if (navigator.msSaveBlob) {
      csvURL = navigator.msSaveBlob(csvData, 'download.csv');
    }
    else {
      csvURL = window.URL.createObjectURL(csvData);
    }

    const tempLink = document.createElement('a');
    tempLink.href = csvURL;
    tempLink.setAttribute('download', 'download.csv');
    tempLink.click();
  }

}
