import { ComponentFactoryResolver, Injectable, ViewContainerRef } from '@angular/core';
import { DynamicComponent } from '@app/core/models/ticket/ticketDestination';
import { TicketDestinationWorkOrderComponent } from './ticket-destination-work-order/ticket-destination-work-order.component';

@Injectable({
  providedIn: 'root'
})
export class TicketDestinationLoaderService {

  rootViewContainer: ViewContainerRef;

  constructor(private factoryResolver: ComponentFactoryResolver) {
  }

  setRootViewContainerRef(viewContainerRef: ViewContainerRef): void {
    this.rootViewContainer = viewContainerRef;
  }

  addDynamicComponent(componentDestinationToLoad, componentData: any = null): void {

    const factory = this.factoryResolver.resolveComponentFactory(componentDestinationToLoad);
    this.rootViewContainer.clear();
    try {
      const component = this.rootViewContainer.createComponent(factory);
      this.rootViewContainer.insert(component.hostView);

      const instance = component.instance as DynamicComponent;
      if (componentData) {
        instance.data = componentData;
      }
    } catch (error) {
      console.log({ error });
    }


  }
}
