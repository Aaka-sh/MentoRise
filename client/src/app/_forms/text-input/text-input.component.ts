import { NgIf } from '@angular/common';
import { Component, input, Self } from '@angular/core';
import {
  ControlValueAccessor,
  FormControl,
  NgControl,
  ReactiveFormsModule,
} from '@angular/forms';

@Component({
  selector: 'app-text-input',
  imports: [NgIf, ReactiveFormsModule],
  templateUrl: './text-input.component.html',
  styleUrl: './text-input.component.css',
})

//ControlValueAccessor lets you integrate your custom component with Angular forms just like built-in controls
export class TextInputComponent implements ControlValueAccessor {
  label = input<string>(''); // Label for the input field
  type = input<string>('text'); // Type of the input field (text, password, etc.)

  constructor(@Self() public ngControl: NgControl) {
    this.ngControl.valueAccessor = this; // Set the value accessor to this component
  }
  writeValue(obj: any): void {}
  registerOnChange(fn: any): void {}
  registerOnTouched(fn: any): void {}
  get control(): FormControl {
    return this.ngControl.control as FormControl;
  }
}
