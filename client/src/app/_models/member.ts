//this interface define the structure of member object
import { Photo } from './photo';

export interface Member {
  id: number;
  userName: string;
  age: number;
  photoUrl: string;
  knownAs: string;
  created: Date;
  lastActive: Date;
  gender: string;
  introduction: string;
  interests: string;
  looingFor: any;
  city: string;
  country: string;
  photos: Photo[];
}
