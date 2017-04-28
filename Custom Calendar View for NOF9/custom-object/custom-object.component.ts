import { FormBuilder } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ObjectComponent, UrlManagerService, ContextService, ColorService, ErrorService, ConfigService , ViewModelFactoryService} from 'nakedobjects.spa';
import { Component } from '@angular/core';
import find from 'lodash/find';

@Component({
  selector: 'app-custom-object',
  templateUrl: './custom-object.component.html',
  styleUrls: ['./custom-object.component.css']
})
export class CustomObjectComponent extends ObjectComponent {

  constructor(activatedRoute: ActivatedRoute,
    urlManager: UrlManagerService,
    context: ContextService,
    viewModelFactory: ViewModelFactoryService,
    colorService: ColorService,
    error: ErrorService,
    formBuilder: FormBuilder,
    configService: ConfigService) {
    super(activatedRoute as any, urlManager, context, viewModelFactory, colorService, error, formBuilder as any, configService)
  }

  customCollection() {
    if (this.object) {
      const collection  = find(this.object.collections, c => c.title === this.customConfig.nameOfCollection);
      return collection;
    }
    return null;
  }

  customConfig = {
    nameOfCollection: 'Work Order Routings',
    startPropertyId: 'ScheduledStartDate',
    endPropertyId: 'ScheduledEndDate',
    allDay: false,
    defaultView: 'month'
  };
  
}
