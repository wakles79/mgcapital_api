import { EntityModel } from "../../common/entity.model";
import { CleaningReportCreateModel } from "./cleaning.report.create.model";

export class CleaningReportGridModel extends CleaningReportCreateModel {

    guid: string;
    number: number;
    preparedFor: string;
    to: string;
    cleaningItems: number;
    findingItems: number;
    companyName: string;
    formattedTo: string;
    submitted: number;
    status: string;
    lastCommentDirection: boolean;

    constructor(entity: CleaningReportGridModel) {
        super(entity);

        this.guid = entity.guid;
        this.number = entity.number || null;
        this.preparedFor = entity.preparedFor;
        this.to = entity.to;
        this.cleaningItems = entity.cleaningItems;
        this.companyName = entity.companyName;
        this.formattedTo = entity.formattedTo;
        this.submitted = entity.submitted;
        this.status = entity.status;
        this.lastCommentDirection = entity.lastCommentDirection;
    }
}
