import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomObjectComponent } from './custom-object.component';

describe('CustomObjectComponent', () => {
  let component: CustomObjectComponent;
  let fixture: ComponentFixture<CustomObjectComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CustomObjectComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CustomObjectComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
