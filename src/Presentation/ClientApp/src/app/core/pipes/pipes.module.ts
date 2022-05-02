import { NgModule } from '@angular/core';
import { FromEpochPipe } from './fromEpoch.pipe';
import { SplitStringByUppercasePipe } from './splitStringByUppercase.pipe';
import { TimeAgoExtendsPipe } from './time-ago-extends.pipe';
import { SafeHtmlPipe } from './safe-html.pipe';

@NgModule({
    declarations: [

        FromEpochPipe,
        SplitStringByUppercasePipe,
        TimeAgoExtendsPipe,
        SafeHtmlPipe,

    ],
    imports     : [],
    exports     : [

        FromEpochPipe,
        SplitStringByUppercasePipe,
        TimeAgoExtendsPipe,
        SafeHtmlPipe,

    ]
})

export class PipesModule
{

}
