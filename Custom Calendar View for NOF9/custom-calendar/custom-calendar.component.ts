import { ChangeDetectionStrategy } from '@angular/core';
import { CollectionViewState } from 'nakedobjects.spa';
import { CollectionViewModel, ContextService, UrlManagerService, ErrorService, ColorService } from 'nakedobjects.spa';
import { Component, OnInit, Input } from '@angular/core';

import { CollectionRepresentation, Link, toUtcDate, CollectionMember, ErrorWrapper } from "nakedobjects.spa/lib/app/models";
import map from 'lodash/map'
import {
  CalendarEvent,
  CalendarEventAction,
  CalendarEventTimesChangedEvent
} from 'angular-calendar';
import {
  startOfDay,
  endOfDay,
  subDays,
  addDays,
  endOfMonth,
  isSameDay,
  isSameMonth,
  addHours
} from 'date-fns';

export interface IConfig {
  nameOfCollection: string,
  startPropertyId: string,
  endPropertyId: string,
  allDay: boolean,
  defaultView: string
}
const colors: any = {
  red: {
    primary: '#ad2121',
    secondary: '#FAE3E3'
  },
  blue: {
    primary: '#1e90ff',
    secondary: '#D1E8FF'
  },
  yellow: {
    primary: '#e3bc08',
    secondary: '#FDF1BA'
  }
};

@Component({
  selector: 'app-custom-calendar',
  templateUrl: './custom-calendar.component.html',
  styleUrls: ['./custom-calendar.component.css']
})
export class CustomCalendarComponent implements OnInit {

  constructor(
    private readonly context: ContextService,
    private readonly urlManager: UrlManagerService,
    private readonly error: ErrorService,
    private readonly color: ColorService,
  ) { }

  @Input()
  collection : CollectionViewModel;

  @Input()
  config : IConfig;

  viewDate : Date;

  view: string = 'month';

  activeDayIsOpen: boolean = false;

  events : CalendarEvent[] = [];

  eventClicked(evt: CalendarEvent) {
      this.urlManager.setItem((evt as any).vm as Link, 1);
      return false;
  }

  dayClicked({ date, events }: { date: Date, events: CalendarEvent[] }): void {

      if (isSameMonth(date, this.viewDate)) {
          if ((isSameDay(this.viewDate, date) && this.activeDayIsOpen === true) || events.length === 0) {
              this.activeDayIsOpen = false;
          } else {
              this.activeDayIsOpen = true;
              this.viewDate = date;
          }
      }
  }

  ngOnInit() {
    const collectionRep = this.collection.collectionRep as CollectionMember;

    this.context.getCollectionDetails(collectionRep, CollectionViewState.Table, false).
            then(details => {
                const items: Link[] = details.value();

                if (items.length === 0) {
                    this.viewDate = new Date(Date.now());
                }

                this.events = map(items, (item: Link) => {
                    const props = item.members();
                    const start = props[this.config.startPropertyId].value();
                    const end = props[this.config.endPropertyId].value();

                    const startDate = toUtcDate(start);
                    const endDate = toUtcDate(end);

                    if (!this.viewDate || startDate > this.viewDate) {
                        this.viewDate = startDate;
                    }

                    return {
                        title: item.title(),
                        start: startDate,
                        end: endDate,
                        allDay: this.config.allDay,
                        color : colors.red,
                        vm: item,
                        url: "empty" //Because NOF manages the navigation -  see below
                    } as CalendarEvent;
                });
            }).
            catch((reject: ErrorWrapper) => this.error.handleError(reject));
  }
}
