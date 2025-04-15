//this component allows a user to edit their profile information
import {
  Component,
  HostListener,
  inject, //modern angular way to inject dependencies
  OnInit, //angular lifecycle hook
  ViewChild,
} from '@angular/core';
import { Member } from '../../_models/member';
import { AccountService } from '../../_services/account.service';
import { MembersService } from '../../_services/member.service';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { FormsModule, NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { PhotoEditorComponent } from '../photo-editor/photo-editor.component';

@Component({
  selector: 'app-member-edit',
  imports: [TabsModule, FormsModule, PhotoEditorComponent],
  templateUrl: './member-edit.component.html',
  styleUrl: './member-edit.component.css',
})
export class MemberEditComponent implements OnInit {
  //reference to the form defined in the template
  @ViewChild('editForm') editForm?: NgForm;
  //prompt the user with a browser alert if they try to leave the page with unsaved changes
  @HostListener('window:beforeunload', ['$event']) notify($event: any) {
    //check if the form has unsaved changes
    if (this.editForm?.dirty) {
      $event.returnValue = true; //this will prompt the user to confirm leaving the page
    }
  }
  member?: Member;
  private accountService = inject(AccountService);
  private memberService = inject(MembersService);
  private toastr = inject(ToastrService);

  ngOnInit(): void {
    //load the member data when the component is initialized
    this.loadMember();
  }

  loadMember() {
    //get the current user from the account service
    const user = this.accountService.currentUser();
    if (!user) return;
    this.memberService.getMember(user.username).subscribe({
      next: (member) => {
        this.member = member;
      },
    });
  }

  updateMember() {
    this.memberService.updateMember(this.editForm?.value).subscribe({
      next: (_) => {
        this.toastr.success('Profile updated successfully!');
        this.editForm?.reset(this.member);
      },
    });
  }

  onMemberChange(event: Member) {
    this.member = event; //update the member object with the new photo
  }
}
